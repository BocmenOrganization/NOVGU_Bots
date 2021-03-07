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
            switch (name)
            {
                case NamePage_Main:
                    return new PageMain();
                case NamePage_Setting:
                    return new PageSetting();
                case NamePage_StartNewUser:
                    return new PageStart();
            }
            return null;
        }
        public Type GetTypePage(string name, IBot.BotTypes? botType = null, string keyBot = null)
        {
            switch (name)
            {
                case NamePage_Main:
                    return typeof(PageMain);
                case NamePage_Setting:
                    return typeof(PageSetting);
                case NamePage_StartNewUser:
                    return typeof(PageStart);
            }
            return null;
        }
    }
}
