﻿using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using BotsCore;
using System;
using BotsCore.Moduls.Translate;
using BotsCore.Moduls.Tables.Interface;

namespace NOVGUBots.App.NOVGU_Standart.Pages
{
    public class PageSetLanguage : Page
    {
        private static readonly ModelMarkerTextData Message_TextStart = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 11);
        private static readonly KitButton Button_MessageLangsList;
        static PageSetLanguage()
        {
            ITableString tableTextButtons = (ITableString)GlobalTableManager.GetTable(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableString);
            Button_MessageLangsList = new(new Button[][]
            {
                new Button[]
                {
                    new Button(tableTextButtons.GetDataTextId(0), (inBot, s, data) => { SetLang(inBot, Lang.LangTypes.ru, data); return true; }),
                },
                new Button[]
                {
                    new Button(tableTextButtons.GetDataTextId(1), (inBot, s, data) => { SetLang(inBot, Lang.LangTypes.en, data); return true; }),
                },
                new Button[]
                {
                    new Button(tableTextButtons.GetDataTextId(2), (inBot, s, data) => { SetLang(inBot, Lang.LangTypes.de, data); return true; })
                }
            });
            static void SetLang(ObjectDataMessageInBot inBot, Lang.LangTypes lang, object data)
            {
                if (inBot.User.Lang != lang)
                {
                    inBot.User.Lang = lang;
                    ManagerPage.ResetSendKeyboard(inBot);
                    foreach (var userbot in inBot.User.BotsAccount)
                    {
                        if (userbot != inBot.BotUser)
                        {
                            ObjectDataMessageInBot botsUserItem = new ObjectDataMessageInBot(inBot.User, userbot);
                            ManagerPage.ResetSendKeyboard(botsUserItem);
                            ManagerPage.ResetSendLastMessage(botsUserItem);
                        }
                    }
                }
                inBot.User.Lang = lang;
                if (!UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.NewUser))
                    ManagerPage.SetBackPage(inBot);
                else
                {
                    ManagerPage.ClearHistoryPage(inBot);
                    ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_StartNewUser);
                }
            }
        }
        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
        {
            SendMessage(inBot);
        }
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!Button_MessageLangsList.CommandInvoke(inBot))
                SendMessage(inBot);
        }
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendMessage(inBot);
        private void SendMessage(ObjectDataMessageInBot inBot) => SendDataBot(new ObjectDataMessageSend(inBot) { TextObj = Message_TextStart, ButtonsMessage = Button_MessageLangsList });
    }
}
