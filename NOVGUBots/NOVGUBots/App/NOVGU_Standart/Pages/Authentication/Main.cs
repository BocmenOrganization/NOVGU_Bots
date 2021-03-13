using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using BotsCore;
using static BotsCore.Bots.Model.ObjectDataMessageSend;
using System.Threading.Tasks;
using System.Linq;
using System;


namespace NOVGUBots.App.NOVGU_Standart.Pages.Authentication
{
    public class Main : Page
    {
        private static readonly ModelMarkerTextData Message_TextStartMain = new(CreatePageAppStandart.NameApp, "MainTextNOVGU", 5);
        private static readonly ModelMarkerTextData Buttons_IdTextStudent = Message_TextStartMain.GetElemNewId(6);
        private static readonly ModelMarkerTextData Buttons_IdTextTeacher = Message_TextStartMain.GetElemNewId(7);
        private static readonly ModelMarkerTextData Message_TextStartCreteUser = Message_TextStartMain.GetElemNewId(8);
        private const uint Buttons_IdTextMainPage = 9;
        private const uint Buttons_IdTextBackPage = 10;
        private static readonly KitButton Buttons_Message = new(new Button[][]
            {
                new Button[]
                {
                    new Button(Buttons_IdTextStudent, (inBot, s, data) => { return true; })
                },
                new Button[]
                {
                    new Button(Buttons_IdTextTeacher, (inBot, s, data) => { return true; })
                }
            });
        public static readonly KitButton Buttons_Keyboard = new(new Button[][]
            {
                new Button[]
                {
                    new Button(Message_TextStartMain.GetElemNewId(Buttons_IdTextBackPage), (inBot, s, data) => { return true; }),
                    new Button(Message_TextStartMain.GetElemNewId(Buttons_IdTextMainPage), (inBot, s, data) => { return true; })
                }
            });

        public bool IsNewUser = false;

        public override void EventInMessage(ObjectDataMessageInBot inBot) => SendMessage(inBot);
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendMessage(inBot);
        public override KitButton GetKeyboardButtons(ObjectDataMessageInBot inBot) => Buttons_Keyboard;
        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
        {
            if (dataOpenPage is bool boolResul)
                IsNewUser = boolResul;
            SendDataBot(new ObjectDataMessageSend(inBot) { ButtonsKeyboard = Buttons_Keyboard }).Wait();
            SendMessage(inBot);
        }
        private void SendMessage(ObjectDataMessageInBot inBot)
        {
            SendDataBot(new ObjectDataMessageSend(inBot) { Text = string.Format(Message_TextStartMain.GetText(inBot), (IsNewUser ? Message_TextStartCreteUser.GetText(inBot) : null), Buttons_IdTextStudent.GetText(inBot), Buttons_IdTextTeacher.GetText(inBot)), ButtonsMessage = Buttons_Message });
        }
    }
}
