using BotsCore.Bots.Model;
using BotsCore.Moduls.Translate;

namespace NOVGUBots.ManagerPageNOVGU.Interface
{
    public interface IGetButtonsSetting
    {
        public (string AppName, string PageName, Text NameButton)[] GetPages(ObjectDataMessageInBot inBot);
    }
}
