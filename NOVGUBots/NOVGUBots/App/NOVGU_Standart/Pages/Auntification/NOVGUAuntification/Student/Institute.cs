using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using NOVGUBots.Moduls.NOVGU_SiteData;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;
using System.Linq;
using BotsCore.Moduls.Translate;
using BotsCore;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification.Student
{
    public class Institute : Page
    {
        public const string NamePage = "NovguUser=Регистрация->Студент-1";
        private static readonly ModelMarkerTextData Message_TextStartMainInstitute = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 31);
        private static readonly ModelMarkerTextData Message_TextStartMainICollege = Message_TextStartMainInstitute.GetElemNewId(32);

        private KitButton MessageButtons;
        public TypePars type;
        public Text[] HistorySet;

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
        {
            type = UserRegister.GetTypeSchedule(inBot);
            HistorySet = new Text[] { DataNOVGU.GetInfoScheduleInstitute(type).Name };
            Start(inBot);
            ResetLastMessenge(inBot);
        }
        public override void EventStoreLoad(ObjectDataMessageInBot inBot, bool state) => Start(inBot);
        private void Start(ObjectDataMessageInBot inBot)
        {
            MessageButtons = KitButton.GenerateKitButtonsTexts(DataNOVGU.GetInfoScheduleInstitute(type).Institute?.Select(x => new Text[] { x.Name }).ToArray(), CommandInvoke, 1d);
        }
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!MessageButtons.CommandInvoke(inBot))
                ResetLastMessenge(inBot);
        }
        private void CommandInvoke(ObjectDataMessageInBot inBot, Text text, object data)
        {
            UserRegister.SetNameInstituteCollege(text.GetDefaultText(), inBot);
            Array.Resize(ref HistorySet, HistorySet.Length + 1);
            HistorySet[HistorySet.Length - 1] = text;
            ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, Course.NamePage, HistorySet);
        }

        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendDataBot(new ObjectDataMessageSend(inBot) { Text = Main.GetTextMainFormat(HistorySet, (type == TypePars.College ? Message_TextStartMainICollege.GetText(inBot) : Message_TextStartMainInstitute.GetText(inBot)), inBot), ButtonsMessage = MessageButtons });
    }
}
