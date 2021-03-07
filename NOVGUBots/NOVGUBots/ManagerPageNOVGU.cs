using BotsCore.Bots.Interface;
using System.Collections.Generic;
using NOVGUBots.SettingCore.Interface;

namespace NOVGUBots
{
    public static class ManagerPageNOVGU
    {
        private static List<IGetButtonsPage> buttonsPages = new List<IGetButtonsPage>();
        private static List<IGetButtonsSetting> buttonsSettings = new List<IGetButtonsSetting>();

        public static void AddApp(object app)
        {
            if (app is ICreatePageApp createPageApp)
                BotsCore.ManagerPage.Add_ICreatePageApp(createPageApp);
            else
                throw new BotsCore.Model.Exception("Данное приложение не содержжит конструктор: ICreatePageApp");
            if (app is IGetButtonsPage buttonsPage)
                buttonsPages.Add(buttonsPage);
            if (app is IGetButtonsSetting buttonsSetting)
                buttonsSettings.Add(buttonsSetting);
        }
    }
}