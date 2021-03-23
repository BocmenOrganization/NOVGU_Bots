using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using BotsCore.Moduls.Translate;
using NOVGUBots.Moduls.NOVGU_SiteData;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using BotsCore;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification
{
    public class ConfirmationUser : Page
    {
        public const string NamePage = "NovguUser=Регистрация->Подтверждение";

        private const string FiledNameDateTime = "DateTime_Page_ConfirmationUser";
        private const string FiledNameCode = "Code_Page_ConfirmationUser";
        private const string FiledNameStatePage = "State_Page_ConfirmationUser";
        private const string FiledNameOldEmail = "EmailSend_Page_ConfirmationUser";
        private const int CountCharCode = 6;
        private const uint TimeTesetSend = 120;
        private static readonly Random random = new Random();

        private static readonly ModelMarkerTextData Message_TextStart = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 39);
        private static readonly ModelMarkerTextData Message_TextTimeInfo = Message_TextStart.GetElemNewId(40);
        private static readonly ModelMarkerTextData Message_TextCodeError = Message_TextStart.GetElemNewId(41);
        private static readonly KitButton MessageButtons = new(new Button[][]
            {
                new Button[]
                {
                    new Button(Message_TextStart.GetElemNewId(42), (inBot, s, data) => { return true; }),
                }
            });

        public Text[] HistorySet;
        public DateTime sendTime;
        public string Code;
        private Task taskMessage;
        private bool isSendingStatusMessage;
        public bool StatePage;
        private string email;

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
        {
            email = GetUser(inBot).Email;
            if (inBot.User[FiledNameOldEmail] is string oldEmail && oldEmail == email)
            {
                if (inBot.User[FiledNameDateTime] is DateTime dateTime)
                    sendTime = dateTime;
                if (inBot.User[FiledNameCode] is string code)
                    Code = code;
                if (inBot.User[FiledNameStatePage] is bool statePage)
                    StatePage = statePage;
            }

            if (dataOpenPage is Text[] texts)
                HistorySet = texts;
            else if (dataOpenPage is JArray valuePairs)
                HistorySet = valuePairs.ToObject<Text[]>();
            Start(inBot);
        }
        public override void EventStoreLoad(ObjectDataMessageInBot inBot, bool state)
        {
            email = GetUser(inBot).Email;
            Start(inBot);
        }
        private void Start(ObjectDataMessageInBot inBot)
        {
            inBot.User[FiledNameOldEmail] = email;
            taskMessage = Task.Run(() =>
            {
                isSendingStatusMessage = true;
                double second;
                SendMail(inBot);
                do
                {
                    second = (DateTime.Now - sendTime).TotalSeconds;
                    SendDataBot(new ObjectDataMessageSend(inBot) { Text = Student.Main.GetTextMainFormat(HistorySet, string.Format(Message_TextStart.GetText(inBot), email, string.Format(Message_TextTimeInfo.GetText(inBot), TimeTesetSend - (int)second)), inBot) });
                    System.Threading.Thread.Sleep(1000);

                } while (second < TimeTesetSend && isSendingStatusMessage);
                if (isSendingStatusMessage)
                    SendDataBot(new ObjectDataMessageSend(inBot) { Text = Student.Main.GetTextMainFormat(HistorySet, string.Format(Message_TextStart.GetText(inBot), email, string.Empty), inBot), ButtonsMessage = MessageButtons });
                StatePage = true;
                inBot.User[FiledNameStatePage] = true;
            });
        }
        private void SendMail(ObjectDataMessageInBot inBot)
        {
            if ((DateTime.Now - sendTime).TotalSeconds >= TimeTesetSend)
            {
                Moduls.NOVGU_SiteData.Model.User user = GetUser(inBot);
                Code = GenerateRandomCode();
                inBot.User[FiledNameCode] = Code;
                sendTime = DateTime.Now;
                inBot.User[FiledNameDateTime] = sendTime;
                AuthorizationMailing.Send(Code, "", email, user.Name, inBot);
            }
        }
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (StatePage && inBot.MessageText == Code)
            {
                StatePage = true;
                inBot.User[FiledNameDateTime] = null;
                inBot.User[FiledNameCode] = null;
                inBot.User[FiledNameStatePage] = null;
                UserRegister.AddFlag(UserRegister.RegisterState.ConnectNovgu, inBot);
                if (UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.NewUser))
                {
                    UserRegister.RemoveFlag(UserRegister.RegisterState.NewUser, inBot);
                    ManagerPage.ClearHistoryPage(inBot);
                    ManagerPage.SetPage(inBot, CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_Main);
                }
            }
            else
                ResetLastMessenge(inBot);
        }
        public override void EventClose(ObjectDataMessageInBot inBot)
        {
            isSendingStatusMessage = false;
            taskMessage?.Wait();
        }

        public override void ResetLastMessenge(ObjectDataMessageInBot inBot)
        {
            EventClose(inBot);
            Start(inBot);
        }

        private static string GenerateRandomCode() => new string(Enumerable.Range(0, CountCharCode + 1).Select(x => (char)random.Next(65, 122)).ToArray());
        private static Moduls.NOVGU_SiteData.Model.User GetUser(ObjectDataMessageInBot inBot)
        {
            string NameIdUser = UserRegister.GetUser(inBot);
            if (UserRegister.GetUserState(inBot) == UserRegister.UserState.Student)
            {
                string NameInstituteCollege = UserRegister.GetNameInstituteCollege(inBot);
                string NameCourse = UserRegister.GetNameCourse(inBot);
                string NameGroup = UserRegister.GetNameGroup(inBot);
                return DataNOVGU.GetInfoScheduleInstitute(UserRegister.GetTypeSchedule(inBot)).Institute?.FirstOrDefault(x => x.Name.GetDefaultText() == NameInstituteCollege)?.Courses?.FirstOrDefault(x => x.Name.GetDefaultText() == NameCourse)?.groups?.FirstOrDefault(x => x.Name == NameGroup)?.users?.FirstOrDefault(x => x.IdString == NameIdUser);
            }
            else
                return DataNOVGU.UserTeachers?.FirstOrDefault(x => x.User.IdString == NameIdUser).User;
        }
    }
}
