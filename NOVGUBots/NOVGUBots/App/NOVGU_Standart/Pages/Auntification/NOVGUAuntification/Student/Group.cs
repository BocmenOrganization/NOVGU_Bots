using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using NOVGUBots.Moduls.NOVGU_SiteData;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;
using System.Linq;
using BotsCore.Moduls.Translate;
using System.Collections.Generic;
using BotsCore;
using Newtonsoft.Json.Linq;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification.Student
{
    public class Group : Page
    {
        public const string NamePage = "NovguUser=Регистрация->Студент-3";
        private static readonly ModelMarkerTextData Message_TextStart = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 34);

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
            string NameCourse = UserRegister.GetNameCourse(inBot);
            var DataGroup = DataNOVGU.GetInfoScheduleInstitute(UserRegister.GetTypeSchedule(inBot)).Institute.FirstOrDefault(x => x.Name.GetDefaultText() == NameInstituteCollege)?.Courses?.FirstOrDefault(x => x.Name.GetDefaultText() == NameCourse)?.groups?.Select(x => x.Name).ToArray();
            MessageButtons = KitButton.GenerateKitButtonsTexts(textsButton(DataGroup), CommandInvoke, 1d);
            static string[][] textsButton(string[] texts) => texts.Select((x, i) => (x, i)).GroupBy<(string, int), int>(x => ((x.Item2 >> 1) << 1)).Select(x => x.Select(y => y.Item1).ToArray()).ToArray();
        }
        private void CommandInvoke(ObjectDataMessageInBot inBot, string text, object data)
        {
            UserRegister.SetNameGroup(text, inBot);
            UserRegister.AddFlag(UserRegister.RegisterState.GroupOrTeacherSet, inBot);
            Array.Resize(ref HistorySet, HistorySet.Length + 1);
            HistorySet[^1] = new Text(inBot, text);
            IsConfirmationUser.SetNextPageStudentOrTeacer(inBot, 4, HistorySet);
        }

        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!MessageButtons.CommandInvoke(inBot))
                ResetLastMessenge(inBot);
        }

        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendDataBot(new ObjectDataMessageSend(inBot) { Text = Main.GetTextMainFormat(HistorySet, Message_TextStart.GetText(inBot), inBot), ButtonsMessage = MessageButtons });
    }
}
