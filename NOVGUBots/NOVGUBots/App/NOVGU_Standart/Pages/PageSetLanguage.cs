using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using BotsCore;
using static BotsCore.Bots.Model.ObjectDataMessageSend;
using System.Threading.Tasks;
using System.Linq;
using System;
using BotsCore.Moduls.Translate;

namespace NOVGUBots.App.NOVGU_Standart.Pages
{
    public class PageSetLanguage : Page
    {
        private static readonly ModelMarkerTextData Message_TextStart = new(CreatePageAppStandart.NameApp, "MainTextNOVGU", 11);

        private static readonly KitButton Button_MessageLangsList;
        static PageSetLanguage()
        {
            Button_MessageLangsList = new(new Button[][]
            {
                new Button[]
                {
                    new Button(Message_TextStart.GetElemNewId(12).GetText(Lang.LangTypes.ru), (inBot, s, data) => { SetLang(inBot, Lang.LangTypes.ru, data); return true; }),
                },
                new Button[]
                {
                    new Button(Message_TextStart.GetElemNewId(13).GetText(Lang.LangTypes.en), (inBot, s, data) => { SetLang(inBot, Lang.LangTypes.en, data); return true; }),
                },
                new Button[]
                {
                    new Button(Message_TextStart.GetElemNewId(14).GetText(Lang.LangTypes.de), (inBot, s, data) => { SetLang(inBot, Lang.LangTypes.de, data); return true; })
                }
            });
            static void SetLang(ObjectDataMessageInBot inBot, Lang.LangTypes lang, object data)
            {
                inBot.User.Lang = lang;
                if (data is bool IsNewUser && IsNewUser)
                    ManagerPage.SetPage(inBot, CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_StartNewUser);
                else
                    ManagerPage.SetBackPage(inBot);
            }
        }
        public bool IsNewUser = false;
        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
        {
            if (dataOpenPage is bool boolResul)
                IsNewUser = boolResul;
            SendMessage(inBot);
        }
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!Button_MessageLangsList.CommandInvoke(inBot, IsNewUser))
                SendMessage(inBot);
        }
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendMessage(inBot);
        public override bool IsSendStandartButtons(ObjectDataMessageInBot inBot) => false;
        public override KitButton GetKeyboardButtons(ObjectDataMessageInBot inBot) => IsNewUser ? PageStart.Keyboard_Further : null;
        private void SendMessage(ObjectDataMessageInBot inBot)
        {
            SendDataBot(new ObjectDataMessageSend(inBot) { TextObj = Message_TextStart, ButtonsMessage = Button_MessageLangsList });
        }
    }
}
