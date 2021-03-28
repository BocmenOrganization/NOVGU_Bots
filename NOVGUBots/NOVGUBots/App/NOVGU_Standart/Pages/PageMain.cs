using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using BotsCore.Moduls.Translate;
using NOVGUBots.Moduls.NOVGU_SiteData;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using BotsCore;

namespace NOVGUBots.ManagerPageNOVGU
{
    public static partial class ManagerPageNOVGU
    {
        public class PageMain : Page
        {
            private KitButton buttons;
            public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
            {
                Start(inBot);
                ResetLastMessenge(inBot);
            }
            public override void EventStoreLoad(ObjectDataMessageInBot inBot, bool state) => Start(inBot);
            public void Start(ObjectDataMessageInBot inBot) => buttons = GetButtonsPages(buttonsPages.Select(x => x.GetPages(inBot)).ToList());
            public override void EventInMessage(ObjectDataMessageInBot inBot)
            {
                if (!buttons.CommandInvoke(inBot))
                    ResetLastMessenge(inBot);
            }
            public override void ResetLastMessenge(ObjectDataMessageInBot inBot)
            {
                SendDataBot(new ObjectDataMessageSend(inBot) { Text = $"{DateTime.Now}  Это главная", ButtonsMessage = buttons });
            }
        }
    }
}
