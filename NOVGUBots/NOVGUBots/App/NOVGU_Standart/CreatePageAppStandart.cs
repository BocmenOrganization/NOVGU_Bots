using BotsCore.Bots.Interface;
using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Moduls.Translate;
using NOVGUBots.App.NOVGU_Standart.Pages;
using NOVGUBots.ManagerPageNOVGU.Interface;
using System;

namespace NOVGUBots.App.NOVGU_Standart
{
    public class CreatePageAppStandart : ICreatePageApp, IGetButtonsSetting
    {
        public const string NameApp = "НовГУ_Main";
        public const string NameTableText = "MainTextNOVGU";
        public const string NameTableString = "MainStringNOVGU";
        public const string NameTableMedia = "MainMediaNOVGU";

        public const string NamePage_Main = "Главная";
        public const string NamePage_Setting = "Настройки";
        public const string NamePage_StartNewUser = "Start";
        public const string NamePage_RegisterMain = "Регистрация->UserBot->Главная";
        private static readonly ModelMarkerTextData ButtonName_SetLanguage = new(NameApp, NameTableText, 46);
        public const string NamePage_SetLanguage = "Выбор языка";
        public string GetNameApp() => NameApp;
        public object GetPage(string name, ObjectDataMessageInBot inBot)
        {
            return name switch
            {
                NamePage_Main => new ManagerPageNOVGU.ManagerPageNOVGU.PageMain(),
                NamePage_Setting => new ManagerPageNOVGU.ManagerPageNOVGU.PageSetting(),
                NamePage_StartNewUser => new PageStart(),
                NamePage_RegisterMain => new Pages.Auntification.Main(),
                NamePage_SetLanguage => new PageSetLanguage(),
                Pages.Auntification.Main.BotsAuntification_Register => new Pages.Auntification.BotsAuntification.Register(),
                Pages.Auntification.Main.BotsAuntification_Entry => new Pages.Auntification.BotsAuntification.Entry(),
                Pages.Auntification.NOVGUAuntification.BindingNOVGU.NamePage => new Pages.Auntification.NOVGUAuntification.BindingNOVGU(),
                Pages.Auntification.NOVGUAuntification.Student.Main.NamePage => new Pages.Auntification.NOVGUAuntification.Student.Main(),
                Pages.Auntification.NOVGUAuntification.Student.Institute.NamePage => new Pages.Auntification.NOVGUAuntification.Student.Institute(),
                Pages.Auntification.NOVGUAuntification.Student.Course.NamePage => new Pages.Auntification.NOVGUAuntification.Student.Course(),
                Pages.Auntification.NOVGUAuntification.Student.Group.NamePage => new Pages.Auntification.NOVGUAuntification.Student.Group(),
                Pages.Auntification.NOVGUAuntification.Student.User.NamePage => new Pages.Auntification.NOVGUAuntification.Student.User(),
                Pages.Auntification.NOVGUAuntification.IsConfirmationUser.NamePage => new Pages.Auntification.NOVGUAuntification.IsConfirmationUser(),
                Pages.Auntification.NOVGUAuntification.ConfirmationUser.NamePage => new Pages.Auntification.NOVGUAuntification.ConfirmationUser(),
                Pages.Auntification.NOVGUAuntification.Teacher.Search.NamePage => new Pages.Auntification.NOVGUAuntification.Teacher.Search(),
                _ => null,
            };
        }

        public (string AppName, string PageName, Text NameButton)[] GetPages(ObjectDataMessageInBot inBot)
        {
            return new (string AppName, string PageName, Text NameButton)[]
            {
                (NameApp, NamePage_SetLanguage, ButtonName_SetLanguage),
                (NameApp, NamePage_RegisterMain, ButtonName_SetLanguage.GetElemNewId(47)),
                (NameApp, Pages.Auntification.NOVGUAuntification.BindingNOVGU.NamePage, ButtonName_SetLanguage.GetElemNewId(48))
            };
        }

        public Type GetTypePage(string name, ObjectDataMessageInBot inBot)
        {
            return name switch
            {
                NamePage_Main => typeof(ManagerPageNOVGU.ManagerPageNOVGU.PageMain),
                NamePage_Setting => typeof(ManagerPageNOVGU.ManagerPageNOVGU.PageSetting),
                NamePage_StartNewUser => typeof(PageStart),
                NamePage_RegisterMain => typeof(Pages.Auntification.Main),
                NamePage_SetLanguage => typeof(PageSetLanguage),
                Pages.Auntification.Main.BotsAuntification_Register => typeof(Pages.Auntification.BotsAuntification.Register),
                Pages.Auntification.Main.BotsAuntification_Entry => typeof(Pages.Auntification.BotsAuntification.Entry),
                Pages.Auntification.NOVGUAuntification.BindingNOVGU.NamePage => typeof(Pages.Auntification.NOVGUAuntification.BindingNOVGU),
                Pages.Auntification.NOVGUAuntification.Student.Main.NamePage => typeof(Pages.Auntification.NOVGUAuntification.Student.Main),
                Pages.Auntification.NOVGUAuntification.Student.Institute.NamePage => typeof(Pages.Auntification.NOVGUAuntification.Student.Institute),
                Pages.Auntification.NOVGUAuntification.Student.Course.NamePage => typeof(Pages.Auntification.NOVGUAuntification.Student.Course),
                Pages.Auntification.NOVGUAuntification.Student.Group.NamePage => typeof(Pages.Auntification.NOVGUAuntification.Student.Group),
                Pages.Auntification.NOVGUAuntification.Student.User.NamePage => typeof(Pages.Auntification.NOVGUAuntification.Student.User),
                Pages.Auntification.NOVGUAuntification.IsConfirmationUser.NamePage => typeof(Pages.Auntification.NOVGUAuntification.IsConfirmationUser),
                Pages.Auntification.NOVGUAuntification.ConfirmationUser.NamePage => typeof(Pages.Auntification.NOVGUAuntification.ConfirmationUser),
                Pages.Auntification.NOVGUAuntification.Teacher.Search.NamePage => typeof(Pages.Auntification.NOVGUAuntification.Teacher.Search),
                _ => null,
            };
        }
    }
}
