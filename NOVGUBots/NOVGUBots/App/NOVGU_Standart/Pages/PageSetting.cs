using BotsCore.Bots.Model;
using BotsCore.Bots.Model.Buttons;
using BotsCore.Moduls.Tables.Services;
using NOVGUBots.App.NOVGU_Standart;
using System;

namespace NOVGUBots.ManagerPageNOVGU
{
    public static partial class ManagerPageNOVGU
    {
        public class PageSetting : Page
        {
            private static readonly ModelMarkerTextData NameButtonOpenSchedule = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 52);
            private KitButton buttons;
            public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
            {
                Start(inBot);
                ResetLastMessenge(inBot);
            }
            public override void EventStoreLoad(ObjectDataMessageInBot inBot, bool state) => Start(inBot);
            public void Start(ObjectDataMessageInBot inBot) => buttons = GetButtonsPage(inBot, false);
            public override void EventInMessageNOVGU(ObjectDataMessageInBot inBot)
            {
                if (!buttons.CommandInvoke(inBot))
                    ResetLastMessenge(inBot);
            }
            public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendDataBot(new ObjectDataMessageSend(inBot) { Text = $"{NameButtonOpenSchedule.GetText(inBot)}\n\n{GetText(inBot, false)}", ButtonsMessage = buttons });
        }
    }
}