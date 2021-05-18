using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using System.Timers;
using static BotsCore.Bots.Model.ObjectDataMessageSend;
using NOVGUBots.App.NOVGU_Standart;
using NOVGUBots.Moduls;

namespace NOVGUBots.ManagerPageNOVGU
{
    public static partial class ManagerPageNOVGU
    {
        public class PageMain : Page
        {
            private static readonly ModelMarkerUneversalData<Media[]> modelMarkerMedia = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableMedia, 4);
            private static readonly ModelMarkerTextData NamePage = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 61);
            public static readonly ModelMarkerTextData TextInfoTimeUpdate = NamePage.GetElemNewId(60);
            private static readonly ModelMarkerTextData TextHelpPage = NamePage.GetElemNewId(70);
            private static readonly TimeSpan myZero = TimeSpan.Parse("23:59:59");

            private KitButton buttons;
            private Timer timerMessage;

            public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage) => Start(inBot);
            public override void EventStoreLoad(ObjectDataMessageInBot inBot, bool state) => Start(inBot);
            public void Start(ObjectDataMessageInBot inBot)
            {
                buttons = GetButtonsPage(inBot, true);
                timerMessage = new();
                timerMessage.Elapsed += (o, e) => ResetLastMessenge(inBot);
                timerMessage.AutoReset = true;
                timerMessage.Interval = 1800000;
                timerMessage.Start();
                ResetLastMessenge(inBot);
            }
            public override void EventInMessageNOVGU(ObjectDataMessageInBot inBot)
            {
                if (!buttons.CommandInvoke(inBot))
                    ResetLastMessenge(inBot);
            }
            private static Media[] GetMedia()
            {
                TimeSpan timeSpanNow = DateTime.Now.TimeOfDay;
                Media resul = null;

                //if (timeSpanNow < ExternalEnvironment.blueHourMorning.start && timeSpanNow > ExternalEnvironment.blueHourNight.end)// Ночь

                if (ExternalEnvironment.blueHourNight.end > TimeSpan.Zero ? 
                    ((timeSpanNow < myZero && timeSpanNow > ExternalEnvironment.blueHourNight.end) || (timeSpanNow > TimeSpan.Zero && timeSpanNow < ExternalEnvironment.blueHourMorning.start)) 
                    : ExternalEnvironment.blueHourNight.end > timeSpanNow && timeSpanNow < ExternalEnvironment.blueHourMorning.start) // Ночь
                    resul = modelMarkerMedia.GetData()[0];
                else if (timeSpanNow > ExternalEnvironment.blueHourMorning.start && timeSpanNow <= ExternalEnvironment.blueHourMorning.end)// Синий час утром
                    resul = modelMarkerMedia.GetData()[1];
                else if (timeSpanNow > ExternalEnvironment.goldenHourMorning.start && timeSpanNow <= ExternalEnvironment.goldenHourMorning.end)// Золотой час утром
                    resul = modelMarkerMedia.GetData()[2];
                else if (timeSpanNow > ExternalEnvironment.goldenHourMorning.end && timeSpanNow <= ExternalEnvironment.goldenHourNight.start)// День
                    resul = modelMarkerMedia.GetData()[3];
                else if (timeSpanNow > ExternalEnvironment.goldenHourNight.start && timeSpanNow <= ExternalEnvironment.goldenHourNight.end)// Золотой час вечером
                    resul = modelMarkerMedia.GetData()[4];
                else if (timeSpanNow > ExternalEnvironment.blueHourNight.start && timeSpanNow <= ExternalEnvironment.blueHourNight.end)// Синий час вечером
                    resul = modelMarkerMedia.GetData()[5];
                if (resul != null)
                    return new Media[] { resul };
                else
                    return null;
            }
            public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendDataBot(new ObjectDataMessageSend(inBot) { Text = $"{NamePage.GetText(inBot)}\n\n{GetText(inBot, true)}{string.Format(TextInfoTimeUpdate.GetText(inBot), DateTime.Now)}", ButtonsMessage = buttons, media = GetMedia() });
            public override (string text, Media[] media)? EventGetHelp(ObjectDataMessageInBot inBot) => (TextHelpPage.GetText(inBot), null);
        }
    }
}
