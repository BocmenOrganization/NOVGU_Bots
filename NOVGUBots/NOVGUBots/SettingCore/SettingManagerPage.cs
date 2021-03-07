using BotsCore.Bots;
using BotsCore.Bots.Interface;
using BotsCore.Bots.Model;
using BotsCore.Bots.Model.Buttons;
using BotsCore.Bots.Model.Buttons.Command;
using BotsCore.Moduls.GetSetting.Interface;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Moduls.Translate;
using NOVGUBots.App.NOVGU_Standart.Pages;
using System;

namespace NOVGUBots.SettingCore
{
    public class SettingManagerPage : ISettingManagerPage
    {
        private ModelMarkerTextData textCreteUser;
        private ModelMarkerTextData textSetButtons;
        public SettingManagerPage(IObjectSetting setting)
        {
            textCreteUser = new ModelMarkerTextData(CreatePageAppStandart.NameApp, setting.GetValue("SettingManagerPage_NameTable"), uint.Parse(setting.GetValue("SettingManagerPage_textCreteUser")));
            textSetButtons = textCreteUser.GetElemNewId(uint.Parse(setting.GetValue("SettingManagerPage_textSetButtons")));
        }
        public (string NameApp, string NamePage) GetPageCreteUser() => (CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_StartNewUser);

        public Action<ObjectDataMessageInBot> GetRegisterMethod() => RegisterUser;

        public CommandList GetSpecialCommand() => null;

        public Button[][] GetStandartButtons()
        {
            throw new NotImplementedException();
        }

        public string GetTextCreteUser(Lang.LangTypes lang) => textCreteUser.GetText(lang);// "Добро пожаловать в нашу экосистему";

        public string GetTextSetButtons(Lang.LangTypes lang) => textSetButtons.GetText(lang);// "Выдаю клавиши";

        public bool SetStandartButtonsCreteUser() => false;


        private void RegisterUser(ObjectDataMessageInBot inBot)
        {
            string textSend = GetTextCreteUser(inBot.User.Lang);
            if (SetStandartButtonsCreteUser())
            {
                if (!string.IsNullOrWhiteSpace(textSend))
                {
                    ManagerBots.SendDataBot(new ObjectDataMessageSend(inBot) { Text = "Тестовый текст 0" });
                }
                else
                {
                    textSend = GetTextSetButtons(inBot.User.Lang);
                    if (!string.IsNullOrWhiteSpace(textSend))
                    {
                        ManagerBots.SendDataBot(new ObjectDataMessageSend(inBot) { Text = "Тестовый текст 1" });
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(textSend))
            {
                ManagerBots.SendDataBot(new ObjectDataMessageSend(inBot) { Text = "Тестовый текст 2" });
            }
            var pageSetInfo = GetPageCreteUser();
            BotsCore.ManagerPage.SetPageSaveHistory(inBot, pageSetInfo.NameApp, pageSetInfo.NamePage);
        }
    }
}