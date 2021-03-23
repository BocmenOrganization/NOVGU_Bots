using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using BotsCore;
using static BotsCore.Bots.Model.ObjectDataMessageSend;
using System.Threading.Tasks;
using System.Linq;
using System;


namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification
{
    public class BindingNOVGU : Page
    {
        public const string NamePage = "NovguUser=Регистрация->Главная";

        private static readonly ModelMarkerTextData Message_TextStartMain = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 5);
        private static readonly ModelMarkerTextData Buttons_IdTextStudent = Message_TextStartMain.GetElemNewId(6);
        private static readonly ModelMarkerTextData Buttons_IdTextTeacher = Message_TextStartMain.GetElemNewId(7);

        private static readonly KitButton Buttons_Message = new(new Button[][]
            {
                new Button[]
                {
                    new Button(Buttons_IdTextStudent, (inBot, s, data) => { ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, Student.Main.NamePage); return true; })
                },
                new Button[]
                {
                    new Button(Buttons_IdTextTeacher, (inBot, s, data) => { ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, Teacher.Search.NamePage); return true; })
                }
            });

        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!Buttons_Message.CommandInvoke(inBot))
                SendMessage(inBot);
        }
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendMessage(inBot);
        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage) => SendMessage(inBot);
        private void SendMessage(ObjectDataMessageInBot inBot)
        {
            SendDataBot(new ObjectDataMessageSend(inBot) { Text = string.Format(Message_TextStartMain.GetText(inBot), Buttons_IdTextStudent.GetText(inBot), Buttons_IdTextTeacher.GetText(inBot)), ButtonsMessage = Buttons_Message });
        }
    }
}
