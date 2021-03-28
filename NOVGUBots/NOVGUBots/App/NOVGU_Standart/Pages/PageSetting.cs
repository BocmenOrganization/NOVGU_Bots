using BotsCore.Bots.Model;
using BotsCore.Bots.Model.Buttons;
using System;
using System.Linq;

namespace NOVGUBots.ManagerPageNOVGU
{
    public static partial class ManagerPageNOVGU
    {
        public class PageSetting : Page
        {
            private KitButton buttons;
            public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
            {
                Start(inBot);
                ResetLastMessenge(inBot);
            }
            public override void EventStoreLoad(ObjectDataMessageInBot inBot, bool state) => Start(inBot);
            public void Start(ObjectDataMessageInBot inBot) => buttons = GetButtonsPages(buttonsSettings.Select(x => x.GetPages(inBot)).ToList());
            public override void EventInMessage(ObjectDataMessageInBot inBot)
            {
                if (!buttons.CommandInvoke(inBot))
                    ResetLastMessenge(inBot);
            }
            public override void ResetLastMessenge(ObjectDataMessageInBot inBot)
            {
                SendDataBot(new ObjectDataMessageSend(inBot) { Text = $"{DateTime.Now}  Это настройки", ButtonsMessage = buttons });
            }
        }
    }
}