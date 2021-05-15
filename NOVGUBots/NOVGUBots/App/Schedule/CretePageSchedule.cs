using BotsCore.Bots.Interface;
using BotsCore.Bots.Model;
using BotsCore.User.Models;
using NOVGUBots.App.Schedule.Pages;
using NOVGUBots.Moduls.NOVGU_SiteData;
using NOVGUBots.Moduls.NOVGU_SiteData.Model.Schedule;
using System;
using NOVGUBots.ManagerPageNOVGU.Interface;
using BotsCore.Moduls.Translate;
using BotsCore.Moduls.Tables.Services;
using System.Linq;

namespace NOVGUBots.App.Schedule
{
    public class CretePageSchedule : ICreatePageApp, IGetButtons, IGetTexts
    {
        public const string NameApp = "ScheduleApp";
        public const string NameTableText = "ScheduleAppText";
        public const string NameMainPage = "MainPage";
        public const string NamePageSchedule = "Schedule";
        public const string NameTableMedia = "ScheduleAppMedia";
        public const string NameTableString = "ScheduleAppString";

        private static readonly ModelMarkerTextData NameButtonOpenSchedule = new(NameApp, NameTableText, 0);
        private static readonly ModelMarkerTextData TitleTextInfoWeek = NameButtonOpenSchedule.GetElemNewId(17);
        private static readonly ModelMarkerTextData FieldNameNumberWeek = NameButtonOpenSchedule.GetElemNewId(18);
        private static readonly (string AppName, string PageName, Text NameButton)[] SetButtonsMain = new (string AppName, string PageName, Text NameButton)[]
        {
            (NameApp, NameMainPage, NameButtonOpenSchedule)
        };

        public string GetNameApp() => NameApp;

        public object GetPage(string name, ObjectDataMessageInBot inBot)
        {
            return name switch
            {
                NameMainPage => (IsScheduleFile(inBot)) ? new MainScheduleFile() : new MainScheduleText(),
                NamePageSchedule => new Pages.Schedule(),
                _ => null
            };
        }
        private static bool IsScheduleFile(ModelUser user) => DataNOVGU.GetScheduleUser(user) is TableScheduleStudents tableStudent && tableStudent.typeSchedule == TableScheduleStudents.TypeSchedule.Files;

        public Type GetTypePage(string name, ObjectDataMessageInBot inBot)
        {
            return name switch
            {
                NameMainPage => (IsScheduleFile(inBot)) ? typeof(MainScheduleFile) : typeof(MainScheduleText),
                NamePageSchedule => typeof(Pages.Schedule),
                _ => null
            };
        }

        public (string AppName, string PageName, Text NameButton)[] GetButtonsPagesSetting(ObjectDataMessageInBot inBot) => null;
        public (string AppName, string PageName, Text NameButton)[] GetButtonsPagesMain(ObjectDataMessageInBot inBot) => SetButtonsMain;

        public (string title, string[] texts)[] GetTextsPageSetting(ObjectDataMessageInBot inBot) => null;
        public (string title, string[] texts)[] GetTextsMainPage(ObjectDataMessageInBot inBot)
        {
            string[] texts = new string[3];
            DateTime monday = StaticData.GetThisMonday();
            texts[0] = $"{monday.ToShortDateString()} - {monday.AddDays(6).ToShortDateString()}";
            texts[2] = StaticData.GetInfo_UpDownWeek() ? StaticData.Message_TextUpWeek.GetText(inBot) : StaticData.Message_TextDownWeek.GetText(inBot);

            if (DataNOVGU.Calendar.Length == 2)
            {
                int indexDayInListWeekday = getIndexDayInListWeekday(DataNOVGU.Calendar[0], DateTime.Now);
                texts[1] = string.Format(FieldNameNumberWeek.GetText(inBot), indexDayInListWeekday == -1 ? (getIndexDayInListWeekday(DataNOVGU.Calendar[1], DateTime.Now) * 2 + 2) : (indexDayInListWeekday * 2 + 1));
                static int getIndexDayInListWeekday(DateTime[][] dateTimes, DateTime searchElem)
                {
                    searchElem = searchElem.Date;
                    for (int i = 0; i < dateTimes.Length; i++)
                        if (dateTimes[i].Length == 2 && dateTimes[i].First() <= searchElem && dateTimes[i].Last() >= searchElem)
                            return i;
                    return -1;
                }
            }
            return new (string title, string[] texts)[] { (TitleTextInfoWeek.GetText(inBot), texts) };
        }
    }
}
