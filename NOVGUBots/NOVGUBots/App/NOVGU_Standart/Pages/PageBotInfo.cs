using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.User;
using NOVGUBots.Moduls;
using NOVGUBots.Moduls.NOVGU_SiteData;
using NOVGUBots.SettingCore;
using System;
using System.Timers;
using static NOVGUBots.ManagerPageNOVGU.ManagerPageNOVGU;

namespace NOVGUBots.App.NOVGU_Standart.Pages
{
    public class PageBotInfo : ManagerPageNOVGU.Page
    {
        public const string NamePage = "BotInfo";
        private static readonly ModelMarkerTextData namePage = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 71);
        private static readonly ModelMarkerTextData textCountAllUser = namePage.GetElemNewId(72);
        private static readonly ModelMarkerTextData textScheduleDateTimeUpdated = namePage.GetElemNewId(73);
        private static readonly ModelMarkerTextData textWeatherDateTimeUpdated = namePage.GetElemNewId(75);
        private static readonly ModelMarkerTextData textSunPositionDateTimeUpdated = namePage.GetElemNewId(76);
        private static readonly ModelMarkerTextData textBotVersion = namePage.GetElemNewId(77);

        private Timer timerMessage;

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage) => Start(inBot);
        public override void EventStoreLoad(ObjectDataMessageInBot inBot, bool state) => Start(inBot);
        private void Start(ObjectDataMessageInBot inBot)
        {
            timerMessage = new();
            timerMessage.Elapsed += (o, e) => ResetLastMessenge(inBot);
            timerMessage.AutoReset = true;
            timerMessage.Interval = 1800000;
            timerMessage.Start();
            ResetLastMessenge(inBot);
        }
        public override void EventInMessageNOVGU(ObjectDataMessageInBot inBot) => ResetLastMessenge(inBot);
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot)
        {
            string s = $"{namePage.GetText(inBot)}\n{string.Format(textBotVersion.GetText(inBot), NOVGUSetting.objectSetting.GetValue("Version"))}\n\n{string.Format(textCountAllUser.GetText(inBot), ManagerUser.CountUser)}\n\n{string.Format(textScheduleDateTimeUpdated.GetText(inBot), DataNOVGU.datetimeUpdatedInf)}\n\n{string.Format(textSunPositionDateTimeUpdated.GetText(inBot), ExternalEnvironment.DateTimeUpdatedSunPosition)}\n\n{string.Format(textWeatherDateTimeUpdated.GetText(inBot), ExternalEnvironment.DateTimeUpdatedWeather)}\n\n{string.Format(PageMain.TextInfoTimeUpdate.GetText(inBot), DateTime.Now)}";
            SendDataBot(new ObjectDataMessageSend(inBot)
            {
                Text = $"{namePage.GetText(inBot)}\n{string.Format(textBotVersion.GetText(inBot), NOVGUSetting.objectSetting.GetValue("Version"))}\n\n{string.Format(textCountAllUser.GetText(inBot), ManagerUser.CountUser)}\n\n{string.Format(textScheduleDateTimeUpdated.GetText(inBot), DataNOVGU.datetimeUpdatedInf)}\n\n{string.Format(textSunPositionDateTimeUpdated.GetText(inBot), ExternalEnvironment.DateTimeUpdatedSunPosition)}\n\n{string.Format(textWeatherDateTimeUpdated.GetText(inBot), ExternalEnvironment.DateTimeUpdatedWeather)}\n\n{string.Format(PageMain.TextInfoTimeUpdate.GetText(inBot), DateTime.Now)}"
            });
        }
    }
}
