using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using NOVGUBots.Moduls.NOVGU_SiteData;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;
using System.Linq;
using BotsCore.Moduls.Translate;
using BotsCore;
using static NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification.BindingNOVGU;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification.Student
{
    public class Institute : ManagerPageNOVGU.Page
    {
        public const string NamePage = "NovguUser=Регистрация->Студент-1";
        private static readonly ModelMarkerTextData Message_TextStartMainInstitute = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 31);
        private static readonly ModelMarkerTextData Message_TextStartMainICollege = Message_TextStartMainInstitute.GetElemNewId(32);

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
            MessageButtons = KitButton.GenerateKitButtonsTexts(DataNOVGU.GetInfoScheduleInstitute(registerInfo.type).Institute?.Select(x => new Text[] { x.Name }).ToArray(), CommandInvoke, 1d);
        }
        public override void EventInMessageNOVGU(ObjectDataMessageInBot inBot)
        {
            if (!MessageButtons.CommandInvoke(inBot))
                ResetLastMessenge(inBot);
        }
        private void CommandInvoke(ObjectDataMessageInBot inBot, Text text, object data)
        {
            registerInfo.NameInstituteColleg = text.GetDefaultText();
            registerInfo.textsHistory.Add(text);
            ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, Course.NamePage, registerInfo);
        }

        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendDataBot(new ObjectDataMessageSend(inBot) { Text = Main.GetTextMainFormat(registerInfo.textsHistory, (registerInfo.type == TypePars.College ? Message_TextStartMainICollege.GetText(inBot) : Message_TextStartMainInstitute.GetText(inBot)), inBot), ButtonsMessage = MessageButtons });
    }
}
