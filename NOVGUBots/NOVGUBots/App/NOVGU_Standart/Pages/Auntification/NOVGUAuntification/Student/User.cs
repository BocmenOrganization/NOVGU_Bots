using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using BotsCore.Moduls.Translate;
using NOVGUBots.Moduls.NOVGU_SiteData;
using System.Linq;
using BotsCore;
using static NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification.BindingNOVGU;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification.Student
{
    public class User : ManagerPageNOVGU.Page
    {
        public const string NamePage = "NovguUser=Регистрация->Студент-4";
        private static readonly ModelMarkerTextData Message_TextStart = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 35);

        private KitButton MessageButtons;
        public RegisterInfo registerInfo;

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
        {
            registerInfo = RegisterInfo.Load(dataOpenPage);
            Start(inBot);
            ResetLastMessenge(inBot);
        }
        public override void EventStoreLoad(ObjectDataMessageInBot inBot, bool state) => Start(inBot);
        private void Start(ObjectDataMessageInBot inBot)
        {
            MessageButtons = KitButton.GenerateKitButtonsTexts(GetUsers(inBot)?.Select(x => new string[] { x.Name }).ToArray(), CommandInvoke, 1d);
        }
        private Moduls.NOVGU_SiteData.Model.User[] GetUsers(ObjectDataMessageInBot inBot)
        {
            string NameInstituteCollege = UserRegister.GetNameInstituteCollege(inBot);
            string NameCourse = UserRegister.GetNameCourse(inBot);
            string NameGroup = UserRegister.GetNameGroup(inBot);
            return IsConfirmationUser.FilterUsers(DataNOVGU.GetInfoScheduleInstitute(UserRegister.GetTypeSchedule(inBot)).Institute?.FirstOrDefault(x => x.Name.GetDefaultText() == NameInstituteCollege)?.Courses?.FirstOrDefault(x => x.Name.GetDefaultText() == NameCourse)?.Groups?.FirstOrDefault(x => x.Name == NameGroup)?.Users);
        }
        private void CommandInvoke(ObjectDataMessageInBot inBot, string text, object data)
        {
            registerInfo.UserId = GetUsers(inBot).FirstOrDefault(x => x.Name == text)?.IdString;
            registerInfo.textsHistory.Add(new Text(inBot, text) { LockTranslator = true });
            ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, ConfirmationUser.NamePage, registerInfo);
        }
        public override void EventInMessageNOVGU(ObjectDataMessageInBot inBot)
        {
            if (!MessageButtons.CommandInvoke(inBot))
                ResetLastMessenge(inBot);
        }

        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendDataBot(new ObjectDataMessageSend(inBot) { Text = Main.GetTextMainFormat(registerInfo.textsHistory, Message_TextStart.GetText(inBot), inBot), ButtonsMessage = MessageButtons });
    }
}