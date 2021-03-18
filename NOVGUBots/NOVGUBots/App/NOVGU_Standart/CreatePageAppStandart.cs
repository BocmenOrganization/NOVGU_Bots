using BotsCore.Bots.Interface;
using NOVGUBots.App.NOVGU_Standart.Pages;
using System;

namespace NOVGUBots.App.NOVGU_Standart
{
    public class CreatePageAppStandart : ICreatePageApp
    {
        public const string NameApp = "НовГУ_Main";
        public const string NameTableText = "MainTextNOVGU";
        public const string NameTableString = "MainStringNOVGU";
        public const string NameTableMedia = "MainMediaNOVGU";

        public const string NamePage_Main = "Главная";
        public const string NamePage_Setting = "Настройки";
        public const string NamePage_StartNewUser = "Start";
        public const string NamePage_RegisterMain = "Регистрация->Главная";
        public const string NamePage_SetLanguage = "Выбор языка";
        public string GetNameApp() => NameApp;
        public object GetPage(string name, IBot.BotTypes? botType = null, string keyBot = null)
        {
            return name switch
            {
                NamePage_Main => null,
                NamePage_Setting => null,
                NamePage_StartNewUser => new PageStart(),
                NamePage_RegisterMain => new Pages.Auntification.Main(),
                NamePage_SetLanguage => new PageSetLanguage(),
                Pages.Auntification.Main.BotsAuntification_Register => new Pages.Auntification.BotsAuntification.Register(),
                Pages.Auntification.Main.BotsAuntification_Entry => new Pages.Auntification.BotsAuntification.Entry(),
                _ => null,
            };
        }
        public Type GetTypePage(string name, IBot.BotTypes? botType = null, string keyBot = null)
        {
            return name switch
            {
                NamePage_Main => null,
                NamePage_Setting => null,
                NamePage_StartNewUser => typeof(PageStart),
                NamePage_RegisterMain => typeof(Pages.Auntification.Main),
                NamePage_SetLanguage => typeof(PageSetLanguage),
                Pages.Auntification.Main.BotsAuntification_Register => typeof(Pages.Auntification.BotsAuntification.Register),
                Pages.Auntification.Main.BotsAuntification_Entry => typeof(Pages.Auntification.BotsAuntification.Entry),
                _ => null,
            };
        }
    }
}
