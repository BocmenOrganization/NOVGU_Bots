using BotsCore;
using BotsCore.Bots.Model;
using BotsCore.Bots.Model.Buttons;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Moduls.Translate;
using BotsCore.User;
using System;
using System.Linq;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.BotsAuntification
{
    public class Register : Page
    {
        /// <summary>
        /// Введите логин - 0
        /// </summary>
        private static readonly ModelMarkerTextData Message_Login = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 21);
        /// <summary>
        /// Введите пароль - 1
        /// </summary>
        private static readonly ModelMarkerTextData Message_Password = Message_Login.GetElemNewId(22);
        /// <summary>
        /// Логин занят - 2
        /// </summary>
        private static readonly ModelMarkerTextData Message_LoginBusy = Message_Login.GetElemNewId(23);
        /// <summary>
        /// Логин не корректен - 3
        /// </summary>
        private static readonly ModelMarkerTextData Message_LoginNonCorrect = Message_Login.GetElemNewId(24);
        /// <summary>
        /// Пароль не корректен - 4
        /// </summary>
        private static readonly ModelMarkerTextData Message_PasswordNonCorrect = Message_Login.GetElemNewId(24);

        /// <summary>
        /// false - требуется ввести логин, true - требуется ввести пароль
        /// </summary>
        public bool StateRegister;
        public byte LastMessageID;
        public string Login;

        public override KitButton GetKeyboardButtons(ObjectDataMessageInBot inBot) => UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.NewUser) ? PageStart.ButtonMessage_NewUser : null;
        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage) => ResetLastMessenge(inBot);
        public override void EventClose(ObjectDataMessageInBot inBot)
        {
            if (inBot.User.Password == null)
                inBot.User.Login = null;
        }
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!StateRegister)
            {
                // Установка логина
                if (CheckCorrectLoginAndPassword(inBot.MessageText))
                {
                    if (CheckLoginBusy(inBot.MessageText))
                    {
                        Login = inBot.MessageText;
                        StateRegister = true;
                        LastMessageID = 1;
                        ResetLastMessenge(inBot);
                    }
                    else
                    {
                        LastMessageID = 2;
                        ResetLastMessenge(inBot);
                    }
                }
                else
                {
                    LastMessageID = 3;
                    ResetLastMessenge(inBot);
                }
            }
            else
            {
                // Установка пароля
                if (CheckCorrectLoginAndPassword(inBot.MessageText))
                {
                    if (CheckLoginBusy(Login))
                    {
                        inBot.User.Login = Login;
                        inBot.User.Password = inBot.MessageText;
                        UserRegister.AddFlag(UserRegister.RegisterState.LoginPasswordSet, inBot);
                        if (UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.NewUser))
                        {
                            ManagerPage.ClearHistoryListPage(inBot, 1);
                            ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, NOVGUAuntification.BindingNOVGU.NamePage);
                        }
                        else
                            ManagerPage.SetBackPage(inBot);
                    }
                }
                else
                {
                    LastMessageID = 4;
                    ResetLastMessenge(inBot);
                }
            }
        }

        public override void ResetLastMessenge(ObjectDataMessageInBot inBot)
        {
            ObjectDataMessageSend objectDataMessageSend = new(inBot);
            objectDataMessageSend.TextObj = GetTextSend();
            Text GetTextSend()
            {
                if (LastMessageID > 4)
                    LastMessageID = 0;
#pragma warning disable CS8509 // Выражение switch обрабатывает не все возможные значения своего типа входных данных (оно не полное).
                return LastMessageID switch
#pragma warning restore CS8509 // Выражение switch обрабатывает не все возможные значения своего типа входных данных (оно не полное).
                {
                    0 => Message_Login,
                    1 => Message_Password,
                    2 => Message_LoginBusy,
                    3 => Message_LoginNonCorrect,
                    4 => Message_PasswordNonCorrect
                };
            }
            SendDataBot(objectDataMessageSend);
        }
        private static bool CheckCorrectLoginAndPassword(string s) => !string.IsNullOrEmpty(s) && s?.FirstOrDefault(x => !(char.IsDigit(x) || char.IsLetter(x))) == '\0';
        private static bool CheckLoginBusy(string s) => ManagerUser.SearchUser((x) => x.Login == s) == null;
    }
}