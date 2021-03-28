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

namespace NOVGUBots.App.Schedule
{
    public class CretePageSchedule : ICreatePageApp, IGetButtonsPage
    {
        public const string NameApp = "ScheduleApp";
        public const string NameTableText = "ScheduleAppText";
        public const string NameMainPage = "MainPage";
        public const string NamePageSchedule = "Schedule";

        private static readonly ModelMarkerTextData NameButtonOpenSchedule = new(NameApp, NameTableText, 0);
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

        public (string AppName, string PageName, Text NameButton)[] GetPages(ObjectDataMessageInBot inBot) => SetButtonsMain;
    }
}
