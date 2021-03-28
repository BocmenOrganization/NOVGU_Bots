using BotsCore.Bots.Interface;
using System.Collections.Generic;
using NOVGUBots.ManagerPageNOVGU.Interface;
using BotsCore.Bots.Model.Buttons;
using BotsCore.Moduls.Translate;
using BotsCore;

namespace NOVGUBots.ManagerPageNOVGU
{
    public static partial class ManagerPageNOVGU
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
        private static KitButton GetButtonsPages(List<(string AppName, string PageName, Text NameButton)[]> creatersPage)
        {
            List<Button[]> buttons = new();
            if (creatersPage != null)
            {
                foreach (var CreaterPage in creatersPage)
                {
                    if (CreaterPage != null)
                    {
                        foreach (var buttonPage in CreaterPage)
                        {
                            buttons.Add(new Button[] { new Button(buttonPage.NameButton, (inBot, s, data) => { ManagerPage.SetPageSaveHistory(inBot, buttonPage.AppName, buttonPage.PageName); return true; }) });
                        }
                    }
                }
            }
            return new KitButton(buttons.ToArray());
        }
    }
}