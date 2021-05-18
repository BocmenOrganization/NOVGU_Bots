using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using BotsCore;
using System;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;
using BotsCore.Moduls.Translate;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification
{
    public class BindingNOVGU : ManagerPageNOVGU.Page
    {
        public const string NamePage = "NovguUser=Регистрация->Главная";

        private static readonly ModelMarkerTextData Message_TextStartMain = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 5);
        public static readonly ModelMarkerTextData Buttons_IdTextStudent = Message_TextStartMain.GetElemNewId(6);
        public static readonly ModelMarkerTextData Buttons_IdTextTeacher = Message_TextStartMain.GetElemNewId(7);

        private static readonly KitButton Buttons_Message = new(new Button[][]
            {
                new Button[]
                {
                    new Button(Buttons_IdTextStudent, (inBot, s, data) => { ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, Student.Main.NamePage, new RegisterInfo() { userState = UserRegister.UserState.Student }); return true; })
                },
                new Button[]
                {
                    new Button(Buttons_IdTextTeacher, (inBot, s, data) => { ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, Teacher.Search.NamePage, new RegisterInfo() { userState = UserRegister.UserState.Teacher }); return true; })
                }
            });

        public override void EventInMessageNOVGU(ObjectDataMessageInBot inBot)
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

        public struct RegisterInfo
        {
            public List<Text> textsHistory;
            public UserRegister.UserState userState;
            public TypePars type;
            public string NameInstituteColleg;
            public string NameCourse;
            public string NameGroup;
            public string UserId;
            public static RegisterInfo Load(object e)
            {
                if (e == null) return new RegisterInfo();
                if (e is RegisterInfo registerInfo)
                    return registerInfo;
                else if (e is JObject valuePairs)
                    return valuePairs.ToObject<RegisterInfo>();
                return new RegisterInfo();
            }
        }
    }
}
