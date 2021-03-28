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
                    new Button(Message_TextStartMain.GetElemNewId(1), (inBot, s, data) => { GetSchedule(inBot, data, DayOfWeek.Monday); return true; }),
                    new Button(Message_TextStartMain.GetElemNewId(4), (inBot, s, data) => { GetSchedule(inBot, data, DayOfWeek.Thursday); return true; })
                },
                new Button[]
                {
                    new Button(Message_TextStartMain.GetElemNewId(2), (inBot, s, data) => { GetSchedule(inBot, data, DayOfWeek.Tuesday); return true; }),
                    new Button(Message_TextStartMain.GetElemNewId(5), (inBot, s, data) => { GetSchedule(inBot, data, DayOfWeek.Friday); return true; })
                },
                new Button[]
                {
                    new Button(Message_TextStartMain.GetElemNewId(3), (inBot, s, data) => { GetSchedule(inBot, data, DayOfWeek.Wednesday); return true; }),
                    new Button(Message_TextStartMain.GetElemNewId(6), (inBot, s, data) => { GetSchedule(inBot, data, DayOfWeek.Saturday); return true; })
                },
                new Button[]
                {
                    new Button(Message_TextStartMain.GetElemNewId(8), (inBot, s, data) => { GetSchedule(inBot, data, null); return true; })
                }
            });
        public DateTime[] Period;

        public static void GetSchedule(ObjectDataMessageInBot inBot, object dateGet, DayOfWeek? dayOfWeek)
        {

        }

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
        {
            DateTime StartDate = DateTime.Now;
            if (dataOpenPage is DateTime dateTimeStart)
                StartDate = dateTimeStart;
            DateTime monday = DateTime.Now.AddDays((-(int)StartDate.DayOfWeek) + 1);
            Period = new DateTime[] { monday, monday.AddDays(5) };
            ResetLastMessenge(inBot);
        }
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!dayWeek.CommandInvoke(inBot))
                ResetLastMessenge(inBot);
        }
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot)
        {
            SendDataBot(new ObjectDataMessageSend(inBot) { Text = string.Format( Message_TextStartMain.GetText(inBot), Period[0].ToShortDateString(), Period[1].ToShortDateString()), ButtonsMessage = dayWeek });
        }
    }
}
