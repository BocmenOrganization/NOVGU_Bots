using BotsCore;
using BotsCore.Bots.BotsModel;
using BotsCore.Bots.Interface;
using BotsCore.Bots.Model;
using BotsCore.Bots.Model.Buttons;
using BotsCore.Bots.Model.Buttons.Command;
using BotsCore.Moduls;
using BotsCore.Moduls.Tables.Services;
using NOVGUBots.App.NOVGU_Standart;
using System;

namespace NOVGUBots.SettingCore
{
    public class SettingManagerPage : ISettingManagerPage
    {
        private static readonly ModelMarkerTextData textSetButtons = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 0);
        public static readonly ModelMarkerTextData textGetHelpPage = textSetButtons.GetElemNewId(68);
        private static readonly CommandList commandListNewUser;
        private static readonly CommandList commandList;
        private static readonly KitButton buttonsDefaut;

        static SettingManagerPage()
        {
            ModelMarkerTextData ButtonTextBack = textSetButtons.GetElemNewId(10);
            ObjectCommand backPage = new((inBot, degreeSimilarity, data) => 
            {
                if (!(inBot.BotUser[ManagerPageNOVGU.Page.NameAppData, ManagerPageNOVGU.Page.NamePageData] is bool stateHelpOpened && stateHelpOpened))
                    ManagerPage.SetBackPage(inBot);
                else
                {
                    inBot.BotUser[ManagerPageNOVGU.Page.NameAppData, ManagerPageNOVGU.Page.NamePageData] = false;
                    ManagerPage.ResetSendLastMessage(inBot);
                }
                return true; 
            }, ButtonTextBack);
            ObjectCommand GetKeyboard = new((inBot, degreeSimilarity, data) =>
            {
                if (UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.NewUser))
                {
                    ManagerPage.SendDataBot(new(inBot) { ButtonsKeyboard = App.NOVGU_Standart.Pages.PageStart.ButtonMessage_NewUser }, false, ManagerPage.GetPageUser(inBot));
                }
                else
                    ManagerPage.ResetSendKeyboard(inBot);
                ManagerPage.ResetSendLastMessage(inBot);
                return true;
            }, "/keyboard");
            ObjectCommand GetLastMessage = new((inBot, degreeSimilarity, data) => { ManagerPage.ResetSendLastMessage(inBot); return true; }, "/lastmessage");

            commandList = new CommandList(new ObjectCommand[]
            {
                backPage,
                GetKeyboard,
                GetLastMessage,
                new ObjectCommand((inBot, s, data) => { ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_Setting); return true; }, textSetButtons.GetElemNewId(50)),
                new ObjectCommand((inBot, s, data) => { ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_Main); return true; },textSetButtons.GetElemNewId(49))
            });
            commandListNewUser = new CommandList(new ObjectCommand[]
            {
                backPage,
                GetKeyboard,
                GetLastMessage
            });
            buttonsDefaut = new(new Button[][]
            {
                new Button[]
                {
                    new Button(textSetButtons.GetElemNewId(50), Ocommand: null),
                    new Button(textSetButtons.GetElemNewId(49), Ocommand: null)
                },
                new Button[]
                {
                    new Button(textGetHelpPage, (inBot, degreeSimilarity, data) => { return true; }),
                    new Button(ButtonTextBack, Ocommand: null)
                },
            });
        }
        public (string NameApp, string NamePage, object SendDataPage) GetPageCreteUser(ObjectDataMessageInBot inBot) => (CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_SetLanguage, true);
        public (string NameApp, string NamePage, object SendDataPage) GetPageNonHistoryPage(ObjectDataMessageInBot inBot)
        {
            if (UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.NewUser))
                return (CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_RegisterMain, null);
            else
                return (CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_Main, null);
        }
        public Action<ObjectDataMessageInBot> GetRegisterMethod(ObjectDataMessageInBot inBot) => RegisterUser;
        public CommandList GetSpecialCommand(ObjectDataMessageInBot inBot) => UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.NewUser) ? commandListNewUser : commandList;
        public Button[][] GetStandartButtons(ObjectDataMessageInBot inBot) => UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.NewUser) ? null : buttonsDefaut;
        public string GetTextCreteUser(ObjectDataMessageInBot inBot) => null;
        public string GetTextSetButtons(ObjectDataMessageInBot inBot) => textSetButtons.GetText(inBot);
        public bool SetStandartButtonsCreteUser(ObjectDataMessageInBot inBot) => false;
        private void RegisterUser(ObjectDataMessageInBot inBot)
        {
            string NameUser = inBot.BotHendler.GetType() == typeof(TelegramBot) ? ((Telegram.Bot.Types.Message)inBot.DataMessenge)?.Chat.Username : null;
            EchoLog.Print($"Добавлен новый пользователь, Id {inBot.BotUser.BotID}{(NameUser != null ? $", Имя: @{NameUser}" : null)}");
        }
    }
}