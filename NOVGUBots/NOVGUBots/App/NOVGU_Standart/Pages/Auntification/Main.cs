using BotsCore;
using BotsCore.Bots.Model;
using BotsCore.Bots.Model.Buttons;
using BotsCore.Moduls.Tables.Services;
using BotsCore.User;
using System;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification
{
    public class Main : Page
    {
        public const string BotsAuntification_Register = "Регистрация->РегистрацияБота";
        public const string BotsAuntification_Entry = "Регистрация->ВходАккБота";
        private static readonly ModelMarkerTextData Message_TextStart = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 12);
        private static readonly KitButton ButtonsMessage_Main = new(new Button[][]
            {
                // Войти
                new Button[]
                {
                    new Button(Message_TextStart.GetElemNewId(16), (inBot, s, data) => { ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, BotsAuntification_Entry, true); return true; })
                },
                // Зарегестрироваться
                new Button[]
                {
                    new Button(Message_TextStart.GetElemNewId(17), (inBot, s, data) => {  ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, BotsAuntification_Register, true); return true; })
                },
                // Отсоеденить акк
                new Button[]
                {
                    new Button(Message_TextStart.GetElemNewId(18), (inBot, s, data) =>
                    {
                        DeliteUser(inBot, false);
                        return true;
                    })
                },
                // Удалить акк
                new Button[]
                {
                    new Button(Message_TextStart.GetElemNewId(19), (inBot, s, data) =>
                    {
                        DeliteUser(inBot, true);
                        return true;
                    })
                },
                // Пропустить
                new Button[]
                {
                    new Button(Message_TextStart.GetElemNewId(20), (inBot, s, data) => { return true; })
                }
            });

        private static void DeliteUser(ObjectDataMessageInBot inBot, bool UserDelite)
        {
            ManagerPage.SendDataBot(new ObjectDataMessageSend(inBot) { ClearOldMessage = true, ClearButtonsKeyboard = true }, false);
            if (UserDelite || inBot.User.BotsAccount?.Count == 1 && !UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.LoginPasswordSet))
                ManagerUser.DeliteUser(inBot.User);
            else
                inBot.User.DeliteUserBot(inBot);
            ManagerPage.SendDataBot(new ObjectDataMessageSend(inBot) { ClearOldMessage = true }, false);
            ManagerPage.InMessageBot(new ObjectDataMessageInBot() { BotHendler = inBot.BotHendler, DataMessenge = inBot.DataMessenge, BotID = inBot.BotID });
        }

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
        {
            SendMessage(inBot);
        }
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!ButtonsMessage_Main.CommandInvoke(inBot))
                SendMessage(inBot);
        }
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendMessage(inBot);
        public override KitButton GetKeyboardButtons(ObjectDataMessageInBot inBot) => UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.NewUser) ? PageStart.ButtonMessage_NewUser : null;

        private void SendMessage(ObjectDataMessageInBot inBot)
        {
            SendDataBot(new ObjectDataMessageSend(inBot) { TextObj = Message_TextStart, ButtonsMessage = GetButtons(inBot) });
        }
        private static Button[][] GetButtons(ObjectDataMessageInBot inBot)
        {
            UserRegister.RegisterState registerState = UserRegister.GetInfoRegisterUser(inBot);
            return ButtonsMessage_Main.GetButtons
                (
                new (byte x, byte y)?[]
                {
                    // Войти
                    registerState.HasFlag(UserRegister.RegisterState.LoginPasswordSet) ? (0, 0) : null,
                    // Зарегестрироваться
                    registerState.HasFlag(UserRegister.RegisterState.LoginPasswordSet) ? (0, 1) : null,
                    // Отсоеденить акк
                    // Это есть у всех
                    // Удалить акк
                    (!registerState.HasFlag(UserRegister.RegisterState.LoginPasswordSet)) ? (0, 3) : null,
                    // Пропустить
                    registerState.HasFlag(UserRegister.RegisterState.LoginPasswordSet) ? (0, 4) : null,
                }
                );
        }
    }
}