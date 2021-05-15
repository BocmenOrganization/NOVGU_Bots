using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using NOVGUBots.Moduls.NOVGU_SiteData;
using System.Linq;
using BotsCore.Moduls.Translate;
using BotsCore;
using static NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification.BindingNOVGU;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification.Student
{
    public class Course : ManagerPageNOVGU.Page
    {
        public const string NamePage = "NovguUser=Регистрация->Студент-2";
        private static readonly ModelMarkerTextData Message_TextStart = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 33);

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
            MessageButtons = KitButton.GenerateKitButtonsTexts(DataNOVGU.GetInfoScheduleInstitute(registerInfo.type).Institute?.FirstOrDefault(x => x.Name.GetDefaultText() == registerInfo.NameInstituteColleg)?.Courses?.Select(x => new Text[] { x.Name }).ToArray(), CommandInvoke, 1d);
        }
        private void CommandInvoke(ObjectDataMessageInBot inBot, Text text, object data)
        {
            registerInfo.NameCourse = text.GetDefaultText();
            registerInfo.textsHistory.Add(text);
            ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, Group.NamePage, registerInfo);
        }
        public override void EventInMessageNOVGU(ObjectDataMessageInBot inBot)
        {
            if (!MessageButtons.CommandInvoke(inBot))
                ResetLastMessenge(inBot);
        }
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendDataBot(new ObjectDataMessageSend(inBot) { Text = Main.GetTextMainFormat(registerInfo.textsHistory, Message_TextStart.GetText(inBot), inBot), ButtonsMessage = MessageButtons });
    }
}
