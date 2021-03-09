using BotsCore;
using BotsCore.Bots.Interface;
using BotsCore.Bots.Model;
using BotsCore.Bots.Model.Buttons;
using BotsCore.Bots.Model.Buttons.Command;
using BotsCore.Moduls.Tables.Services;
using NOVGUBots.App.NOVGU_Standart.Pages;
using System;

namespace NOVGUBots.SettingCore
{
    public class SettingManagerPage : ISettingManagerPage
    {
        private static readonly ModelMarkerTextData textSetButtons = new(CreatePageAppStandart.NameApp, "MainTextNOVGU", 0);


        public (string NameApp, string NamePage) GetPageCreteUser(ObjectDataMessageInBot inBot) => (CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_StartNewUser);
        public Action<ObjectDataMessageInBot> GetRegisterMethod(ObjectDataMessageInBot inBot) => RegisterUser;
        public CommandList GetSpecialCommand(ObjectDataMessageInBot inBot) => null;
        public Button[][] GetStandartButtons(ObjectDataMessageInBot inBot) => null;
        public string GetTextCreteUser(ObjectDataMessageInBot inBot) => null;
        public string GetTextSetButtons(ObjectDataMessageInBot inBot) => textSetButtons.GetText(inBot);
        public bool SetStandartButtonsCreteUser(ObjectDataMessageInBot inBot) => false;
        private void RegisterUser(ObjectDataMessageInBot inBot) { }
    }
}