using BotsCore.Bots.Model;

namespace NOVGUBots.ManagerPageNOVGU.Interface
{
    public interface IGetTexts
    {
        public (string title, string[] texts)[] GetTextsPageSetting(ObjectDataMessageInBot inBot);
        public (string title, string[] texts)[] GetTextsMainPage(ObjectDataMessageInBot inBot);
    }
}