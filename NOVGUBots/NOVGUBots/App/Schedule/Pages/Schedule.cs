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
    public class Schedule : Page
    {
        private static readonly ModelMarkerTextData Message_TextStartMain = new(CretePageSchedule.NameApp, CretePageSchedule.NameTableText, 7);

        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            throw new NotImplementedException();
        }

        public override void ResetLastMessenge(ObjectDataMessageInBot inBot)
        {
            throw new NotImplementedException();
        }
    }
}
