using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using BotsCore;
using static BotsCore.Bots.Model.ObjectDataMessageSend;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace NOVGUBots.App.NOVGU_Standart.Pages
{
    public class PageStart : Page
    {
        private static readonly ModelMarkerUneversalData<Media[]> Message_MedaiMessage = new(CreatePageAppStandart.NameApp, "MainMediaNOVGU", 0);
        private static readonly ModelMarkerTextData Message_TextStart = new(CreatePageAppStandart.NameApp, "MainTextNOVGU", 1);
        private static readonly ModelMarkerTextData Buttons_Text = Message_TextStart.GetElemNewId(2);
        private static readonly ModelMarkerTextData Message_TextSpam = Message_TextStart.GetElemNewId(3);
        private static readonly ModelMarkerTextData Message_TextNext = Message_TextStart.GetElemNewId(4);
        public static readonly KitButton Keyboard_Further = new(new Button[][]
            {
                new Button[]
                {
                    new Button(Buttons_Text, (inBot, s, data) => { ManagerPage.SetPage(inBot, CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_RegisterMain, true); return true; })
                }
            });

        private DateTime timeStartSendMessage;
        public bool IsSendStartMessage;
        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage) => StartMessage(inBot);
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!Keyboard_Further.CommandInvoke(inBot))
                StartMessage(inBot);
        }
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => StartMessage(inBot);
        public override KitButton GetKeyboardButtons(ObjectDataMessageInBot inBot) => Keyboard_Further;
        private void StartMessage(ObjectDataMessageInBot inBot)
        {
            Task.Run(() =>
            {
                if (!IsSendStartMessage)
                {
                    DateTime startMessage = DateTime.Now;
                    timeStartSendMessage = startMessage;

                    string[] texts = Message_TextStart.GetText().GetText(inBot).Split('.').Select(x => x + ".").ToArray();
                    string textResul = string.Empty;
                    object messengeSendInfo = null;
                    for (int i = 0; i < texts.Length; i++)
                    {
                        if (timeStartSendMessage == startMessage)
                        {
                            textResul += texts[i];
                            (_, messengeSendInfo) = SendDataBot(new ObjectDataMessageSend(inBot) { Text = textResul, IsSaveInfoMessenge = false, MessageEditObject = messengeSendInfo }).Result;
                            if ((i + 1) != texts.Length)
                                System.Threading.Thread.Sleep(1500);
                        }
                        else
                        {
                            SendDataBot(new ObjectDataMessageSend(inBot) { TextObj = Message_TextSpam, IsSaveInfoMessenge = false, MessageEditObject = messengeSendInfo }).Wait();
                            return;
                        }
                    }
                    IsSendStartMessage = true;
                }
                SendDataBot(new ObjectDataMessageSend(inBot) { Text = string.Format(Message_TextNext.GetText().GetText(inBot), Buttons_Text.GetText().GetText(inBot)), media = Message_MedaiMessage, IsEditOldMessage = false, IsSaveInfoMessenge = false, ButtonsKeyboard = Keyboard_Further }).Wait();
            });
        }
    }
}