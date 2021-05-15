using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using NOVGUBots.App.NOVGU_Standart;
using NOVGUBots.SettingCore;
using static BotsCore.Bots.Model.ObjectDataMessageSend;

namespace NOVGUBots.ManagerPageNOVGU
{
    public abstract class Page : BotsCore.Bots.Model.Page
    {
        public const string NameAppData = "PageNOVGUInfo";
        public const string NamePageData = "HelpOpened";
        private static readonly ModelMarkerTextData defaultText = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 69);
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (inBot.MessageText?.ToLower() != SettingManagerPage.textGetHelpPage.GetText(inBot).ToLower())
                EventInMessageNOVGU(inBot);
            else
            {
                inBot.BotUser[NameAppData, NamePageData] = true;
                var res = EventGetHelp(inBot);
                if (res != null)
                    SendDataBot(new ObjectDataMessageSend(inBot) { Text = res.Value.text, media = res.Value.media });
                else
                    SendDataBot(new ObjectDataMessageSend(inBot) { TextObj = defaultText });
            }
        }
        public abstract void EventInMessageNOVGU(ObjectDataMessageInBot inBot);
        public virtual (string text, Media[] media)? EventGetHelp(ObjectDataMessageInBot inBot) => null;
    }
}