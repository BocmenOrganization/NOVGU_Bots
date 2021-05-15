using BotsCore.Bots.Interface;
using System.Collections.Generic;
using NOVGUBots.ManagerPageNOVGU.Interface;
using BotsCore.Bots.Model.Buttons;
using BotsCore.Moduls.Translate;
using BotsCore;
using BotsCore.Bots.Model;
using System.Text;
using BotsCore.Moduls.Tables.Services;
using NOVGUBots.App.Schedule;

namespace NOVGUBots.ManagerPageNOVGU
{
    public static partial class ManagerPageNOVGU
    {
        //private static readonly List<IGetButtonsPage> buttonsPages = new();
        //private static readonly List<IGetButtons> buttonsSettings = new();

        private static readonly ModelMarkerStringlData Emoji_newLineAppend = new(CretePageSchedule.NameApp, CretePageSchedule.NameTableString, 15);
        private static readonly ModelMarkerStringlData Emoji_newLineDown = Emoji_newLineAppend.GetElemNewId(16);
        private static readonly ModelMarkerStringlData Emoji_newLineUp = Emoji_newLineAppend.GetElemNewId(17);

        private static readonly List<IGetButtons> buttons = new();
        private static readonly List<IGetTexts> texts = new();

        public static void AddApp(object app)
        {
            if (app is ICreatePageApp createPageApp)
                ManagerPage.Add_ICreatePageApp(createPageApp);
            else
                throw new BotsCore.Model.Exception("Данное приложение не содержжит конструктор: ICreatePageApp");

            if (app is IGetButtons getButtons)
                buttons.Add(getButtons);
            if (app is IGetTexts getTexts)
                texts.Add(getTexts);
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="infoWhatPage">false - setting, true - main</param>
        private static KitButton GetButtonsPage(ObjectDataMessageInBot inBot, bool infoWhatPage)
        {
            List<Button[]> buttons = new();
            foreach (var buttonsPageHandler in ManagerPageNOVGU.buttons)
            {
                var buttonsPage = infoWhatPage ? buttonsPageHandler.GetButtonsPagesMain(inBot) : buttonsPageHandler.GetButtonsPagesSetting(inBot);
                if (buttonsPage != null)
                {
                    foreach (var (AppName, PageName, NameButton) in buttonsPage)
                    {
                        buttons.Add(new Button[] { new Button(NameButton, (inBot, s, data) => { ManagerPage.SetPageSaveHistory(inBot, AppName, PageName); return true; }) });
                    }
                }
            }
            return new KitButton(buttons.ToArray());
        }
        private static string GetText(ObjectDataMessageInBot inBot, bool infoWhatPage)
        {
            StringBuilder stringBuilder = null;
            foreach (var textsHandler in texts)
            {
                if (stringBuilder != null)
                    stringBuilder.Append("\n");
                else
                    stringBuilder = new();

                var textsPages = infoWhatPage ? textsHandler.GetTextsMainPage(inBot) : textsHandler.GetTextsPageSetting(inBot);
                if (textsPages != null)
                {
                    foreach (var elem in textsPages)
                    {
                        if (elem != default)
                        {
                            int i = 0;
                            if (!string.IsNullOrWhiteSpace(elem.title))
                                stringBuilder.AppendLine($"{elem.title}");

                            for (; i < elem.texts.Length; i++)
                                stringBuilder.AppendLine($"{(i < (elem.texts.Length - 1) ? Emoji_newLineAppend : Emoji_newLineDown)}{elem.texts[i]}");

                            stringBuilder.Append('\n');
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
}