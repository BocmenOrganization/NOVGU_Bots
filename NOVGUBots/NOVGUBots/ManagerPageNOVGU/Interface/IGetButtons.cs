using BotsCore.Bots.Model;
using BotsCore.Moduls.Translate;

namespace NOVGUBots.ManagerPageNOVGU.Interface
{
    public interface IGetButtons
    {
        /// <summary>
        /// Получение кнопок для главной страницы
        /// </summary>
        /// <param name="inBot"></param>
        /// <returns></returns>
        public (string AppName, string PageName, Text NameButton)[] GetButtonsPagesSetting(ObjectDataMessageInBot inBot);
        /// <summary>
        /// Получение кнопок для страницы настроек
        /// </summary>
        /// <param name="inBot"></param>
        /// <returns></returns>
        public (string AppName, string PageName, Text NameButton)[] GetButtonsPagesMain(ObjectDataMessageInBot inBot);
    }
}