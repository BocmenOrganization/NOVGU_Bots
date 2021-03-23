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
        private static readonly ModelMarkerTextData textSetButtons = new(CreatePageAppStandart.NameApp, "MainTextNOVGU", 0);
        private static readonly CommandList commandListNewUser;
        private static readonly CommandList commandList;

        static SettingManagerPage()
        {
            ObjectCommand backPage = new ObjectCommand((inBot, degreeSimilarity, data) => { ManagerPage.SetBackPage(inBot); return true; }, textSetButtons.GetElemNewId(10));
            ObjectCommand GetKeyboard = new ObjectCommand((inBot, degreeSimilarity, data) => { ManagerPage.ResetSendKeyboard(inBot); ManagerPage.ResetSendLastMessage(inBot); return true; }, "/keyboard");
            ObjectCommand GetLastMessage = new ObjectCommand((inBot, degreeSimilarity, data) => { ManagerPage.ResetSendLastMessage(inBot); return true; }, "/lastmessage"); ;
            commandListNewUser = new CommandList(new ObjectCommand[]
            {
                backPage,
                GetKeyboard,
                GetLastMessage
            });
            commandList = new CommandList(new ObjectCommand[]
            {
                backPage,
                GetKeyboard,
                GetLastMessage
            });
        }
        public (string NameApp, string NamePage, object SendDataPage) GetPageCreteUser(ObjectDataMessageInBot inBot) => (CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_SetLanguage, true);
        public (string NameApp, string NamePage, object SendDataPage) GetPageNonHistoryPage(ObjectDataMessageInBot inBot)
        {
            if (UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.NewUser))
            {
                return (CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_RegisterMain, null);
            }
            //TODO указать главную страницу (в разработке)
            return default;
        }
        public Action<ObjectDataMessageInBot> GetRegisterMethod(ObjectDataMessageInBot inBot) => RegisterUser;
        public CommandList GetSpecialCommand(ObjectDataMessageInBot inBot)
        {
            if (UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.NewUser) && (inBot.BotUser.Page.NameApp == CreatePageAppStandart.NameApp && inBot.BotUser.Page.NamePage != CreatePageAppStandart.NamePage_SetLanguage && inBot.BotUser.Page.NamePage != CreatePageAppStandart.NamePage_StartNewUser))
                return commandListNewUser;
            else if (UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.GroupSet))
                return commandList;
            else
                return null;
        }
        public Button[][] GetStandartButtons(ObjectDataMessageInBot inBot) => null;
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