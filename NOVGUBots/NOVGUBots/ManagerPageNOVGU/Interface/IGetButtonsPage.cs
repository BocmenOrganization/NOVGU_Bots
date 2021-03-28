using BotsCore.Bots.Model;
using BotsCore.Moduls.Translate;

namespace NOVGUBots.ManagerPageNOVGU.Interface
{
    public interface IGetButtonsPage
    {
        public (string AppName, string PageName, Text NameButton)[] GetPages(ObjectDataMessageInBot inBot);
    }
}
