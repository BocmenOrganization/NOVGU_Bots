using BotsCore.Bots.Interface;
using System;
using static NOVGUBots.App.NOVGU_Standart.Pages.AppNovgu;

namespace NOVGUBots.App.NOVGU_Standart.Pages
{
    public class CreatePageAppStandart : ICreatePageApp
    {
        public const string NameApp = "НовГУ_Main";
        public const string NamePage_Main = "Главная";
        public const string NamePage_Setting = "Настройки";
        public const string NamePage_StartNewUser = "Start";
        public const string NamePage_RegisterMain = "Регистрация->Главная";
        public string GetNameApp() => NameApp;
        public object GetPage(string name, IBot.BotTypes? botType = null, string keyBot = null)
        {
            return name switch
            {
                NamePage_Main => new PageMain(),
                NamePage_Setting => new PageSetting(),
                NamePage_StartNewUser => new PageStart(),
                _ => null,
            };
        }
        public Type GetTypePage(string name, IBot.BotTypes? botType = null, string keyBot = null)
        {
            return name switch
            {
                NamePage_Main => typeof(PageMain),
                NamePage_Setting => typeof(PageSetting),
                NamePage_StartNewUser => typeof(PageStart),
                _ => null,
            };
        }
    }
}
