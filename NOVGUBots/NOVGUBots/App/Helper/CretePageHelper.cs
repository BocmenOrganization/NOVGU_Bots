using BotsCore.Bots.Interface;
using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Moduls.Translate;
using NOVGUBots.App.Helper.Pages;
using NOVGUBots.ManagerPageNOVGU.Interface;
using System;

namespace NOVGUBots.App.Helper
{
    public class CretePageHelper : ICreatePageApp, IGetButtons
    {
        public const string NameApp = "HelpApp";
        public const string NameTableText = "HelpAppText";
        public const string NameMainPage = "MainPage";

        private static readonly ModelMarkerTextData NameButton = new(NameApp, NameTableText, 0);

        public string GetNameApp() => NameApp;

        public object GetPage(string name, ObjectDataMessageInBot inBot)
        {
            return name switch
            {
                NameMainPage => new MainPage(),
                _ => null
            };
        }

        public Type GetTypePage(string name, ObjectDataMessageInBot inBot)
        {
            return name switch
            {
                NameMainPage => typeof(MainPage),
                _ => null
            };
        }
        public (string AppName, string PageName, Text NameButton)[] GetButtonsPagesSetting(ObjectDataMessageInBot inBot) => null;
        public (string AppName, string PageName, Text NameButton)[] GetButtonsPagesMain(ObjectDataMessageInBot inBot) => new (string AppName, string PageName, Text NameButton)[] { (NameApp, NameMainPage, NameButton) };
    }
}
