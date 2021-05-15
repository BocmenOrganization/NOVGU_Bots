using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using NOVGUBots.Moduls.NOVGU_SiteData;
using System.Linq;
using System.Threading.Tasks;
using BotsCore;
using static NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification.BindingNOVGU;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification
{
    public class ConfirmationUser : ManagerPageNOVGU.Page
    {
        private const string CharsCode = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890";
        public const string NamePage = "NovguUser=Регистрация->Подтверждение";

        private const string FiledNameDateTime = "DateTime_Page_ConfirmationUser";
        private const string FiledNameCode = "Code_Page_ConfirmationUser";
        private const string FiledNameStatePage = "State_Page_ConfirmationUser";
        private const string FiledNameOldEmail = "EmailSend_Page_ConfirmationUser";
        private const int CountCharCode = 6;
        private const uint TimeTesetSend = 120;
        private static readonly Random random = new();

        private static readonly ModelMarkerTextData Message_TextStart = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 39);
        private static readonly ModelMarkerTextData Message_TextTimeInfo = Message_TextStart.GetElemNewId(40);
        private static readonly KitButton MessageButtons = new(new Button[][]
            {
                new Button[]
                {
                    new Button(Message_TextStart.GetElemNewId(42), (inBot, s, data) => { return true; }),
                }
            });

        public DateTime sendTime;
        public string Code;
        private Task taskMessage;
        private bool isSendingStatusMessage;
        public bool StatePage;
        private string email;
        public RegisterInfo registerInfo;
        public string userId;

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
        {
            registerInfo = RegisterInfo.Load(dataOpenPage);
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
                    SendDataBot(new ObjectDataMessageSend(inBot) { Text = Student.Main.GetTextMainFormat(registerInfo.textsHistory, string.Format(Message_TextStart.GetText(inBot), email, string.Format(Message_TextTimeInfo.GetText(inBot), TimeTesetSend - (int)second)), inBot) });
                    System.Threading.Thread.Sleep(1000);

                } while (second < TimeTesetSend && isSendingStatusMessage);
                if (isSendingStatusMessage)
                    SendDataBot(new ObjectDataMessageSend(inBot) { Text = Student.Main.GetTextMainFormat(registerInfo.textsHistory, string.Format(Message_TextStart.GetText(inBot), email, string.Empty), inBot), ButtonsMessage = MessageButtons });
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
                StatePage = true;
            }
        }
        public override void EventInMessageNOVGU(ObjectDataMessageInBot inBot)
        {
            if (StatePage && inBot.MessageText == Code)
            {
                EventClose(inBot);
                inBot.User[FiledNameDateTime] = null;
                inBot.User[FiledNameCode] = null;
                inBot.User[FiledNameStatePage] = null;
                inBot.User[FiledNameOldEmail] = null;
                UserRegister.SetRegisterInfo(registerInfo, inBot);
                UserRegister.AddFlag(UserRegister.RegisterState.ConnectNovgu, inBot);
                UserRegister.SetUser(userId, inBot);
                if (UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.NewUser))
                {
                    UserRegister.RemoveFlag(UserRegister.RegisterState.NewUser, inBot);
                    ManagerPage.ClearHistoryPage(inBot);
                    ManagerPage.ResetSendKeyboard(inBot);
                    ManagerPage.SetPage(inBot, CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_Main);
                }
                else
                {
                    ManagerPage.ClearHistoryListPage(inBot, 1);
                    ManagerPage.SetBackPage(inBot);
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
        private static string GenerateRandomCode() => new(Enumerable.Range(0, CountCharCode + 1).Select(x => CharsCode[random.Next(0, CharsCode.Length)]).ToArray());
        private Moduls.NOVGU_SiteData.Model.User GetUser(ObjectDataMessageInBot inBot)
        {
            Moduls.NOVGU_SiteData.Model.User user;
            if (registerInfo.userState == UserRegister.UserState.Student)
            {
                user = DataNOVGU.GetInfoScheduleInstitute(registerInfo.type).Institute?.FirstOrDefault(x => x.Name.GetDefaultText() == registerInfo.NameInstituteColleg)?.Courses?.FirstOrDefault(x => x.Name.GetDefaultText() == registerInfo.NameCourse)?.Groups?.FirstOrDefault(x => x.Name == registerInfo.NameGroup)?.Users?.FirstOrDefault(x => x.IdString == registerInfo.UserId);
            }
            else
                user = DataNOVGU.UserTeachers?.FirstOrDefault(x => x.User.IdString == registerInfo.UserId).User;
            userId = user?.IdString;
            return user;
        }
    }
}
