using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using BotsCore.Moduls.Translate;
using NOVGUBots.Moduls.NOVGU_SiteData;
using System.Linq;
using BotsCore;
using static BotsCore.Bots.Model.ObjectDataMessageSend;
using System.Collections.Generic;

namespace NOVGUBots.App.Schedule.Pages
{
    public class MainScheduleText : Page
    {
        private static readonly ModelMarkerTextData Message_TextStartMain = new(CretePageSchedule.NameApp, CretePageSchedule.NameTableText, 7);
        private static readonly ModelMarkerTextData Message_TextErrorPeriod = Message_TextStartMain.GetElemNewId(9);
        private static readonly KitButton dayWeek = new(new Button[][]
            {
                new Button[]
                {
                    new Button(StaticData.DayOfWeek[0], (inBot, s, data) => { GetSchedule(inBot, data, DayOfWeek.Monday); return true; }),
                    new Button(StaticData.DayOfWeek[3], (inBot, s, data) => { GetSchedule(inBot, data, DayOfWeek.Thursday); return true; })
                },
                new Button[]
                {
                    new Button(StaticData.DayOfWeek[1], (inBot, s, data) => { GetSchedule(inBot, data, DayOfWeek.Tuesday); return true; }),
                    new Button(StaticData.DayOfWeek[4], (inBot, s, data) => { GetSchedule(inBot, data, DayOfWeek.Friday); return true; })
                },
                new Button[]
                {
                    new Button(StaticData.DayOfWeek[2], (inBot, s, data) => { GetSchedule(inBot, data, DayOfWeek.Wednesday); return true; }),
                    new Button(StaticData.DayOfWeek[5], (inBot, s, data) => { GetSchedule(inBot, data, DayOfWeek.Saturday); return true; })
                },
                new Button[]
                {
                    new Button(Message_TextStartMain.GetElemNewId(8), (inBot, s, data) => { GetSchedule(inBot, data, null); return true; })
                }
            });
        public DateTime[] Period;

        public static void GetSchedule(ObjectDataMessageInBot inBot, object dateGet, DayOfWeek? dayOfWeek)
        {
            if (dateGet is DateTime[] period)
            {
                if (DateTime.Now.Date >= period.First().Date && DateTime.Now.Date <= period.Last().Date)
                {
                    if (dayOfWeek != null)
                        ManagerPage.SetPageSaveHistory(inBot, CretePageSchedule.NameApp, CretePageSchedule.NamePageSchedule, new DateTime[] { period.First().AddDays((int)dayOfWeek - 1) });
                    else
                        ManagerPage.SetPageSaveHistory(inBot, CretePageSchedule.NameApp, CretePageSchedule.NamePageSchedule, GeerateDatePeriod(period));
                }
                else
                    ((MainScheduleText)ManagerPage.GetPageUser(inBot)).ErrorPeriod(inBot);
            }
        }
        private static DateTime[] GeerateDatePeriod(DateTime[] period)
        {
            List<DateTime> resul = new List<DateTime>() { period.First() };
            while (period.Last().Date > resul.Last().Date)
                resul.Add(resul.Last().Date.AddDays(1));
            return resul.ToArray();
        }

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
        {
            DateTime StartDate = DateTime.Now;
            if (dataOpenPage is DateTime dateTimeStart)
                StartDate = dateTimeStart;
            DateTime monday = StaticData.GetThisMonday(StartDate);
            Period = new DateTime[] { monday, monday.AddDays(6) };
            ResetLastMessenge(inBot);
        }
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!dayWeek.CommandInvoke(inBot, Period))
                ResetLastMessenge(inBot);
        }
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendMessageDayOfWeek(inBot);
        public void ErrorPeriod(ObjectDataMessageInBot inBot) => SendMessageDayOfWeek(inBot, Message_TextErrorPeriod.GetText(inBot));
        private void SendMessageDayOfWeek(ObjectDataMessageInBot inBot, string dopText = null)
        {
            bool info_UpDownWeek = StaticData.GetInfo_UpDownWeek();
            DayOfWeek[] dates = StaticData.FilterDays(inBot, GeerateDatePeriod(Period))?.Select(x => x.DayOfWeek).ToArray();
            SendDataBot(new ObjectDataMessageSend(inBot) 
            { 
                Text = dates.Length > 0 ? $"{dopText}{string.Format(Message_TextStartMain.GetText(inBot), (info_UpDownWeek ? StaticData.Message_TextUpWeek.GetText(inBot) : StaticData.Message_TextDownWeek.GetText(inBot)), Period[0].ToShortDateString(), Period[1].ToShortDateString())}" : StaticData.Text_NoDataSchedule.GetText(inBot),
                ButtonsMessage = dates.Length > 0 ? dayWeek : null,
                media = info_UpDownWeek ? StaticData.MessageMedia_UpWeek.GetData() : StaticData.MessageMedia_DownWeek.GetData()
            });
        }
    }
}
