using BotsCore.Bots.Interface;
using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Interface;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Moduls.Translate;
using NOVGUBots.App.NOVGU_Standart.Pages;
using NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification;
using NOVGUBots.ManagerPageNOVGU.Interface;
using NOVGUBots.Moduls;
using NOVGUBots.Moduls.NOVGU_SiteData;
using System;

namespace NOVGUBots.App.NOVGU_Standart
{
    public class CreatePageAppStandart : ICreatePageApp, IGetButtons, IGetTexts
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
        private static readonly ModelMarkerTextData TitleProfileSettingInfo = ButtonName_SetLanguage.GetElemNewId(51);
        private static readonly ModelMarkerTextData TitleBotSettingSettingInfo = ButtonName_SetLanguage.GetElemNewId(53);
        private static readonly ModelMarkerTextData TextRegisterStateTitle = ButtonName_SetLanguage.GetElemNewId(54);
        private static readonly ModelMarkerTextData TextNoRegister = ButtonName_SetLanguage.GetElemNewId(55);
        private static readonly ModelMarkerTextData TextRegisterOk = ButtonName_SetLanguage.GetElemNewId(56);
        private static readonly ModelMarkerTextData TextRegisterLoginInfo = ButtonName_SetLanguage.GetElemNewId(57);
        private static readonly ModelMarkerTextData TextRegisterConnectCountInfo = ButtonName_SetLanguage.GetElemNewId(58);
        private static readonly ModelMarkerTextData TextLangInfo = ButtonName_SetLanguage.GetElemNewId(59);

        private static readonly ModelMarkerTextData TitleWeather = ButtonName_SetLanguage.GetElemNewId(64);
        private static readonly ModelMarkerTextData WeatherTemperature = ButtonName_SetLanguage.GetElemNewId(65);
        private static readonly ModelMarkerTextData WeatherHumidity = ButtonName_SetLanguage.GetElemNewId(66);
        private static readonly ModelMarkerTextData WeatherWindSpeed = ButtonName_SetLanguage.GetElemNewId(67);
        private static readonly ModelMarkerStringlData[] WeatherEmoji = new ModelMarkerStringlData[]
            {
                new ModelMarkerStringlData(NameApp, NameTableString, 5),
                new ModelMarkerStringlData(NameApp, NameTableString, 6),
                new ModelMarkerStringlData(NameApp, NameTableString, 7),
                new ModelMarkerStringlData(NameApp, NameTableString, 8),
                new ModelMarkerStringlData(NameApp, NameTableString, 9),
            };

        public const string NamePage_SetLanguage = "Выбор языка";

        private static readonly (string AppName, string PageName, Text NameButton)[] SetButtonsSetting = new (string AppName, string PageName, Text NameButton)[]
        {
            (NameApp, NamePage_SetLanguage, ButtonName_SetLanguage),
            (NameApp, NamePage_RegisterMain, ButtonName_SetLanguage.GetElemNewId(47)),
            (NameApp, BindingNOVGU.NamePage, ButtonName_SetLanguage.GetElemNewId(48))
        };

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

        public (string AppName, string PageName, Text NameButton)[] GetButtonsPagesSetting(ObjectDataMessageInBot inBot) => SetButtonsSetting;
        public (string AppName, string PageName, Text NameButton)[] GetButtonsPagesMain(ObjectDataMessageInBot inBot) => null;

        public (string title, string[] texts)[] GetTextsPageSetting(ObjectDataMessageInBot inBot) => new (string title, string[] texts)[] { GetTextsLangInfo(inBot), GetTextsRegisterInfo(inBot), GetTextsProfileInfo(inBot) };
        private static (string title, string[] texts) GetTextsProfileInfo(ObjectDataMessageInBot inBot)
        {
            if (UserRegister.GetUserState(inBot) == UserRegister.UserState.Student)
            {
                bool isConnectNovgu = UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.ConnectNovgu);
                string[] registerInf = new string[5 + (isConnectNovgu ? 1 : 0)];
                registerInf[0] = BindingNOVGU.Buttons_IdTextStudent.GetText(inBot);
                registerInf[1] = DataNOVGU.GetInfoScheduleInstitute(UserRegister.GetTypeSchedule(inBot)).Name.GetText(inBot);
                registerInf[2] = UserRegister.GetNameInstituteCollege(inBot);
                registerInf[3] = UserRegister.GetNameCourse(inBot);
                registerInf[4] = UserRegister.GetNameGroup(inBot);
                if (isConnectNovgu)
                    registerInf[5] = UserRegister.GetUser(inBot);
                return (TitleProfileSettingInfo.GetText(inBot), registerInf);
            }
            else
                return (TitleProfileSettingInfo.GetText(inBot), new string[] { UserRegister.GetUser(inBot) });
        }
        private static (string title, string[] texts) GetTextsRegisterInfo(ObjectDataMessageInBot inBot)
        {
            bool isRegisterUser = UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.LoginPasswordSet);
            string[] resul = new string[isRegisterUser ? 3 : 1];
            resul[0] = string.Format(TextRegisterStateTitle.GetText(inBot), isRegisterUser ? TextRegisterOk.GetText(inBot) : TextNoRegister.GetText(inBot));
            if (isRegisterUser)
            {
                resul[1] = string.Format(TextRegisterLoginInfo.GetText(inBot), inBot.User.Login);
                resul[2] = string.Format(TextRegisterConnectCountInfo.GetText(inBot), inBot.User.BotsAccount.Count);
            }
            return (TitleBotSettingSettingInfo.GetText(inBot), resul);
        }
        private static (string title, string[] texts) GetTextsLangInfo(ObjectDataMessageInBot inBot) => (TextLangInfo.GetText(inBot), new string[] { (inBot.User.Lang == Lang.LangTypes.ru ? PageSetLanguage.ButtonTextRu.GetText(inBot) : (inBot.User.Lang == Lang.LangTypes.en ? PageSetLanguage.ButtonTextEn.GetText(inBot) : PageSetLanguage.ButtonTextDe.GetText(inBot))) });
        public (string title, string[] texts)[] GetTextsMainPage(ObjectDataMessageInBot inBot)
        {
            string[] texts = new string[]
            {
                string.Format(WeatherTemperature.GetText(inBot), ExternalEnvironment.TempFeelsLike),
                string.Format(WeatherHumidity.GetText(inBot), ExternalEnvironment.Humidity),
                string.Format(WeatherWindSpeed.GetText(inBot), ExternalEnvironment.WindSpeed)
            };
            return new (string title, string[] texts)[] { (string.Format(TitleWeather.GetText(inBot), getWeatherEmoji()), texts) };
            static string getWeatherEmoji()
            {
                if (ExternalEnvironment.Clouds < 0.15f)
                    return WeatherEmoji[0];
                else if (ExternalEnvironment.Clouds >= 0.15f && ExternalEnvironment.Clouds < 0.3f)
                    return WeatherEmoji[1];
                else if(ExternalEnvironment.Clouds >= 0.3f && ExternalEnvironment.Clouds < 0.5f)
                    return WeatherEmoji[2];
                else if (ExternalEnvironment.Clouds >= 0.5f && ExternalEnvironment.Clouds < 0.8f)
                    return WeatherEmoji[3];
                else
                    return WeatherEmoji[4];
            }
        }
    }
}
