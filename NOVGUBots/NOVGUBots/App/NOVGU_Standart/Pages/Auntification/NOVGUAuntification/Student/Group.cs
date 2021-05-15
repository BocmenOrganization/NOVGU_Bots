using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using NOVGUBots.Moduls.NOVGU_SiteData;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;
using System.Linq;
using BotsCore.Moduls.Translate;
using System.Collections.Generic;
using BotsCore;
using Newtonsoft.Json.Linq;
using static NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification.BindingNOVGU;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification.Student
{
    public class Group : ManagerPageNOVGU.Page
    {
        public const string NamePage = "NovguUser=Регистрация->Студент-3";
        private static readonly ModelMarkerTextData Message_TextStart = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 34);

        private KitButton MessageButtons;
        public RegisterInfo registerInfo;

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
        {
            registerInfo = RegisterInfo.Load(dataOpenPage);
            Start(inBot);
            ResetLastMessenge(inBot);
        }
        public override void EventStoreLoad(ObjectDataMessageInBot inBot, bool state) => Start(inBot);
        private void Start(ObjectDataMessageInBot inBot)
        {
            var DataGroup = DataNOVGU.GetInfoScheduleInstitute(registerInfo.type).Institute.FirstOrDefault(x => x.Name.GetDefaultText() == registerInfo.NameInstituteColleg)?.Courses?.FirstOrDefault(x => x.Name.GetDefaultText() == registerInfo.NameCourse)?.Groups?.Select(x => x.Name).ToArray();
            MessageButtons = KitButton.GenerateKitButtonsTexts(textsButton(DataGroup), CommandInvoke, 1d);
            static string[][] textsButton(string[] texts) => texts.Select((x, i) => (x, i)).GroupBy<(string, int), int>(x => ((x.Item2 >> 1) << 1)).Select(x => x.Select(y => y.Item1).ToArray()).ToArray();
        }
        private void CommandInvoke(ObjectDataMessageInBot inBot, string text, object data)
        {
            registerInfo.NameGroup = text;
            registerInfo.textsHistory.Add(new Text(inBot, text) { LockTranslator = true });
            IsConfirmationUser.SetNextPageStudentOrTeacer(inBot, 4, registerInfo);
        }

        public override void EventInMessageNOVGU(ObjectDataMessageInBot inBot)
        {
            if (!MessageButtons.CommandInvoke(inBot))
                ResetLastMessenge(inBot);
        }

        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendDataBot(new ObjectDataMessageSend(inBot) { Text = Main.GetTextMainFormat(registerInfo.textsHistory, Message_TextStart.GetText(inBot), inBot), ButtonsMessage = MessageButtons });
    }
}
