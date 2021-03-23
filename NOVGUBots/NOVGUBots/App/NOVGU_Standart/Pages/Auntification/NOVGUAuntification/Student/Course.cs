using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using NOVGUBots.Moduls.NOVGU_SiteData;
using System.Linq;
using BotsCore.Moduls.Translate;
using BotsCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification.Student
{
    public class Course : Page
    {
        public const string NamePage = "NovguUser=Регистрация->Студент-2";
        private static readonly ModelMarkerTextData Message_TextStart = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 33);

        private KitButton MessageButtons;
        public Text[] HistorySet;

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
        {
            if (dataOpenPage is Text[] texts)
                HistorySet = texts;
            else if (dataOpenPage is JArray valuePairs)
                HistorySet = valuePairs.ToObject<Text[]>();
            Start(inBot);
            ResetLastMessenge(inBot);
        }
        public override void EventStoreLoad(ObjectDataMessageInBot inBot, bool state) => Start(inBot);
        private void Start(ObjectDataMessageInBot inBot)
        {
            string NameInstituteCollege = UserRegister.GetNameInstituteCollege(inBot);
            MessageButtons = KitButton.GenerateKitButtonsTexts(DataNOVGU.GetInfoScheduleInstitute(UserRegister.GetTypeSchedule(inBot)).Institute?.FirstOrDefault(x => x.Name.GetDefaultText() == NameInstituteCollege)?.Courses?.Select(x => new Text[] { x.Name }).ToArray(), CommandInvoke, 1d);
        }
        private void CommandInvoke(ObjectDataMessageInBot inBot, Text text, object data)
        {
            UserRegister.SetNameCourse(text.GetDefaultText(), inBot);
            Array.Resize(ref HistorySet, HistorySet.Length + 1);
            HistorySet[HistorySet.Length - 1] = text;
            ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, Group.NamePage, HistorySet);
        }
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!MessageButtons.CommandInvoke(inBot))
                ResetLastMessenge(inBot);
        }
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendDataBot(new ObjectDataMessageSend(inBot) { Text = Main.GetTextMainFormat(HistorySet, Message_TextStart.GetText(inBot), inBot), ButtonsMessage = MessageButtons });
    }
}
