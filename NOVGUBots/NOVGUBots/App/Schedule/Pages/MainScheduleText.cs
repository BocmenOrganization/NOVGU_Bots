using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using BotsCore.Moduls.Translate;
using NOVGUBots.Moduls.NOVGU_SiteData;
using System.Linq;
using BotsCore;

namespace NOVGUBots.App.Schedule.Pages
{
    public class MainScheduleText : Page
    {
        private static readonly ModelMarkerTextData Message_TextStartMain = new(CretePageSchedule.NameApp, CretePageSchedule.NameTableText, 7);
        private static readonly KitButton dayWeek = new(new Button[][]
            {
                new Button[]
                {
                    new Button(Message_TextStartMain.GetElemNewId(1), (inBot, s, data) => { return true; }),
                    new Button(Message_TextStartMain.GetElemNewId(4), (inBot, s, data) => { return true; })
                },
                new Button[]
                {
                    new Button(Message_TextStartMain.GetElemNewId(2), (inBot, s, data) => { return true; }),
                    new Button(Message_TextStartMain.GetElemNewId(5), (inBot, s, data) => { return true; })
                },
                new Button[]
                {
                    new Button(Message_TextStartMain.GetElemNewId(3), (inBot, s, data) => { return true; }),
                    new Button(Message_TextStartMain.GetElemNewId(6), (inBot, s, data) => { return true; })
                },
                new Button[]
                {
                    new Button(Message_TextStartMain.GetElemNewId(8), (inBot, s, data) => { return true; })
                }
            });
        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage) => ResetLastMessenge(inBot);
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!dayWeek.CommandInvoke(inBot))
                ResetLastMessenge(inBot);
        }

        public override void ResetLastMessenge(ObjectDataMessageInBot inBot)
        {
            SendDataBot(new ObjectDataMessageSend(inBot) { TextObj = Message_TextStartMain, ButtonsMessage = dayWeek });
        }
    }
}
