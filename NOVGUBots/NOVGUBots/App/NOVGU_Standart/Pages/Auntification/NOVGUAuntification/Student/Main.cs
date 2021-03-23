using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using NOVGUBots.Moduls.NOVGU_SiteData;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;
using BotsCore;
using BotsCore.Moduls.Translate;
using System.Linq;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification.Student
{
    public class Main : Page
    {
        public const string NamePage = "NovguUser=Регистрация->Студент-0";
        public static readonly ModelMarkerStringlData StrPatch = new ModelMarkerStringlData(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableString, 3);
        public static readonly ModelMarkerStringlData FormatPatch = StrPatch.GetElemNewId(4);

        private static readonly ModelMarkerTextData Message_TextStartMain = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 30);
        private static readonly KitButton kitButton;

        static Main()
        {
            kitButton = new(new Button[][]
            {
                new Button[]
                {
                    new Button(DataNOVGU.InstituteFullTime.Name, (inBot, s, data) => { SetType(inBot, TypePars.InstituteFullTime); return true; })
                },
                new Button[]
                {
                    new Button(DataNOVGU.InstituteInAbsentia.Name, (inBot, s, data) => { SetType(inBot, TypePars.InstituteInAbsentia); return true; })
                },
                new Button[]
                {
                    new Button(DataNOVGU.College.Name, (inBot, s, data) => { SetType(inBot, TypePars.College); return true; })
                }
            });
            static void SetType(ObjectDataMessageInBot inBot, TypePars type)
            {
                UserRegister.SetTypeSchedule(type, inBot);
                ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, Institute.NamePage);
            }
        }
        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage) => SendMessage(inBot);
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!kitButton.CommandInvoke(inBot))
                SendMessage(inBot);
        }
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendMessage(inBot);
        private void SendMessage(ObjectDataMessageInBot inBot) => SendDataBot(new ObjectDataMessageSend(inBot) { TextObj = Message_TextStartMain, ButtonsMessage = kitButton });
        public static string GetTextMainFormat(Text[] history, string Adddata, Lang.LangTypes langTypes) => string.Format(FormatPatch, history != null ? string.Join(StrPatch, history.Select(x => x.GetText(langTypes))) : string.Empty, Adddata);
    }
}
