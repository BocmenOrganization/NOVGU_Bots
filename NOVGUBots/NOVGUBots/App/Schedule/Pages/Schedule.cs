using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using System;
using BotsCore.Moduls.Translate;
using BotsCore;
using static NOVGUBots.Moduls.NOVGU_SiteData.Model.Schedule.Hendler;

namespace NOVGUBots.App.Schedule.Pages
{
    public class Schedule : ManagerPageNOVGU.Page
    {
        private static readonly ModelMarkerTextData Message_TextStartMain = new(CretePageSchedule.NameApp, CretePageSchedule.NameTableText, 7);
        public DateTime[] dates;
        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
        {
            if (dataOpenPage is DateTime[] dates)
            {
                this.dates = dates;
                ResetLastMessenge(inBot);
            }
            else
                ManagerPage.SetBackPage(inBot);
        }

        public override void EventInMessageNOVGU(ObjectDataMessageInBot inBot) => ResetLastMessenge(inBot);

        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendDataBot(StaticData.GetSendMessage(inBot, dates));
    }
}
