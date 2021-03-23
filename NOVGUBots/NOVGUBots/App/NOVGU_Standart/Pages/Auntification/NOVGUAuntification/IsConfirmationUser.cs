using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using BotsCore.Moduls.Translate;
using Newtonsoft.Json.Linq;
using BotsCore;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification
{
    public class IsConfirmationUser : Page
    {
        public const string NamePage = "NovguUser=Регистрация->Подтверждение?";
        private static readonly ModelMarkerTextData Message_TextStart = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 36);
        private static readonly KitButton MessageButtons = new(new Button[][]
            {
                new Button[]
                {
                    new Button(Message_TextStart.GetElemNewId(37), (inBot, s, data) => { ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, UserRegister.GetUserState(inBot) == UserRegister.UserState.Student ? Student.User.NamePage : ConfirmationUser.NamePage, data); return true; }),
                },
                new Button[]
                {
                    new Button(Message_TextStart.GetElemNewId(38), (inBot, s, data) => { ManagerPage.ClearHistoryPage(inBot); UserRegister.RemoveFlag(UserRegister.RegisterState.NewUser, inBot); ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_Main); return true; }),
                }
            });

        public Text[] HistorySet;

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
        {
            if (dataOpenPage is Text[] texts)
                HistorySet = texts;
            else if (dataOpenPage is JArray valuePairs)
                HistorySet = valuePairs.ToObject<Text[]>();
            ResetLastMessenge(inBot);
        }
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!MessageButtons.CommandInvoke(inBot, HistorySet))
                ResetLastMessenge(inBot);
        }

        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendDataBot(new ObjectDataMessageSend(inBot) { Text = Student.Main.GetTextMainFormat(HistorySet, Message_TextStart.GetText(inBot), inBot), ButtonsMessage = MessageButtons });
    }
}
