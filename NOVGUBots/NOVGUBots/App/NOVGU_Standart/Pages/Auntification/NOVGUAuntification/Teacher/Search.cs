using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using BotsCore.Moduls.Translate;
using NOVGUBots.Moduls.NOVGU_SiteData;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using BotsCore;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification.Teacher
{
    public class Search : Page
    {
        public const string NamePage = "NovguUser=Регистрация->Преподователь";
        private static readonly ModelMarkerTextData Message_TextStart = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 43);
        private static readonly ModelMarkerTextData Message_TextClickListElem = Message_TextStart.GetElemNewId(44);

        public char[] NameUser;
        private KitButton searchUsersButton;

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage) => ResetLastMessenge(inBot);
        public override void EventStoreLoad(ObjectDataMessageInBot inBot, bool state)
        {
            SearchUserDataChar(NameUser);
        }
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!searchUsersButton?.CommandInvoke(inBot) ?? true)
            {
                SearchUser(inBot);
                ResetLastMessenge(inBot);
            }
        }

        public void SearchUser(ObjectDataMessageInBot inBot)
        {
            if (!string.IsNullOrWhiteSpace(inBot.MessageText))
            {
                char[] info = GetWords(inBot.MessageText)?.Select(x => x.First())?.ToArray();
                SearchUserDataChar(info);
            }
        }
        private void SearchUserDataChar(char[] info)
        {
            if (info != null && info.Any())
            {
                var dataSearch = DataNOVGU.UserTeachers.Where(x => Enumerable.SequenceEqual(GetWords(x.User.Name).Select(x => x.First()), info))?.ToArray();
                if (dataSearch?.Any() ?? false)
                {
                    searchUsersButton = KitButton.GenerateKitButtonsTexts(dataSearch.Select(x => new string[] { x.User.Name }).ToArray(), CommandInvoke, 1d);
                    NameUser = info;
                }
            }
        }
        private static string[] GetWords(string text) => text.ToLower().Split(' ')?.Where(x => !string.IsNullOrWhiteSpace(x))?.ToArray();
        private void CommandInvoke(ObjectDataMessageInBot inBot, string text, object data)
        {

        }

        public override void ResetLastMessenge(ObjectDataMessageInBot inBot)
        {
            if (NameUser == null)
                SendDataBot(new ObjectDataMessageSend(inBot) { TextObj = Message_TextStart });
            else
                SendDataBot(new ObjectDataMessageSend(inBot) { TextObj = Message_TextClickListElem, ButtonsMessage = searchUsersButton });
        }
    }
}
