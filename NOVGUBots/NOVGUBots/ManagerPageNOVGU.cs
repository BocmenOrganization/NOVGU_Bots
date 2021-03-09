using BotsCore.Bots.Interface;
using System.Collections.Generic;
using NOVGUBots.SettingCore.Interface;

namespace NOVGUBots
{
    public static class ManagerPageNOVGU
    {
        private static readonly List<IGetButtonsPage> buttonsPages = new();
        private static readonly List<IGetButtonsSetting> buttonsSettings = new();

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