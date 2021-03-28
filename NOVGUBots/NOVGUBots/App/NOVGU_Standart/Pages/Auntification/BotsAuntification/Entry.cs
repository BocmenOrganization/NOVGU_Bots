using BotsCore;
using BotsCore.Bots.Model;
using BotsCore.Bots.Model.Buttons;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Moduls.Translate;
using BotsCore.User;
using BotsCore.User.Models;
using System;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.BotsAuntification
{
    public class Entry : Page
    {
        /// <summary>
        /// Введите логин - 0
        /// </summary>
        private static readonly ModelMarkerTextData Message_Login = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 25);
        /// <summary>
        /// Нет пользователя с таким логином - 1
        /// </summary>
        private static readonly ModelMarkerTextData Message_LoginNon = Message_Login.GetElemNewId(26);
        /// <summary>
        /// Введите пароль - 2
        /// </summary>
        private static readonly ModelMarkerTextData Message_Password = Message_Login.GetElemNewId(27);
        /// <summary>
        /// Неверный пароль - 3
        /// </summary>
        private static readonly ModelMarkerTextData Message_PasswordNon = Message_Login.GetElemNewId(28);
        /// <summary>
        /// Кнопка восстановления пароля
        /// </summary>
        private static KitButton kitButton = new(new Button[][]
            {
                new Button[]
                {
                    new Button(Message_Login.GetElemNewId(29), (inBot, s, data) => { return true; })
                }
            });

        public bool StateAuntification;
        public byte LastMessageID;
        public string Login;

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage) => ResetLastMessenge(inBot);
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!StateAuntification)
            {
                if (!string.IsNullOrWhiteSpace(inBot.MessageText) && ManagerUser.SearchUser((x) => x.Login == inBot.MessageText) != default)
                {
                    Login = inBot.MessageText;
                    StateAuntification = true;
                    LastMessageID = 2;
                    ResetLastMessenge(inBot);
                }
                else
                {
                    LastMessageID = 1;
                    ResetLastMessenge(inBot);
                }
            }
            else
            {
                ModelUser user = ManagerUser.SearchUser((x) => (x.Password == Login) && (x.Password == inBot.MessageText));
                if (!string.IsNullOrWhiteSpace(inBot.MessageText) && user != null)
                {
                    user.AddModelBotUser(inBot);
                    user.LoadToDataBD();
                    inBot.User.DeliteUserBot(inBot.BotID);
                    if (inBot.User.Lang != user.Lang)
                        ManagerPage.ResetSendKeyboard(new ObjectDataMessageInBot(user, inBot.BotUser));
                    inBot.User.Lang = user.Lang;
                    ManagerPage.SetBackPage(inBot);
                }
                else
                {
                    LastMessageID = 3;
                    ResetLastMessenge(inBot);
                }
            }
        }
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot)
        {
            ObjectDataMessageSend objectDataMessageSend = new(inBot);
            objectDataMessageSend.TextObj = GetTextSend();
            if (LastMessageID == 3 && UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.ConnectNovgu))
                objectDataMessageSend.ButtonsMessage = kitButton;
            Text GetTextSend()
            {
                if (LastMessageID > 3)
                    LastMessageID = 0;
#pragma warning disable CS8509 // Выражение switch обрабатывает не все возможные значения своего типа входных данных (оно не полное).
                return LastMessageID switch
#pragma warning restore CS8509 // Выражение switch обрабатывает не все возможные значения своего типа входных данных (оно не полное).
                {
                    0 => Message_Login,
                    1 => Message_LoginNon,
                    2 => Message_Password,
                    3 => Message_PasswordNon,
                };
            }
            SendDataBot(objectDataMessageSend);
        }
        public override KitButton GetKeyboardButtons(ObjectDataMessageInBot inBot) => UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.NewUser) ? PageStart.ButtonMessage_NewUser : null;
    }
}
