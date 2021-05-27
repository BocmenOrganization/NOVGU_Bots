using BotsCore.Bots.Model;
using BotsCore.Bots.Model.Buttons;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Moduls.Translate;
using SearchCore;
using SearchCore.TextEditor.MyStemYandex;
using System;
using System.Linq;

namespace NOVGUBots.App.Helper.Pages
{
    public class MainPage : ManagerPageNOVGU.Page
    {
        private static MyStemProcess myStemProcess = new();
        private static readonly ModelMarkerTextData Message_TextStartMain = new(CretePageHelper.NameApp, CretePageHelper.NameTableText, 1);
        private static readonly ModelMarkerTextData Message_TextQuestionSelect = Message_TextStartMain.GetElemNewId(2);
        private static readonly ModelMarkerTextData Button_TextBack = Message_TextStartMain.GetElemNewId(3);
        private static readonly SearchBotModel[] searchBotModels = new SearchBotModel[]
        {
            new SearchBotModel(new ModelSearch(new ModelWord[][] { new ModelWord[] { new ModelWord("эволюция", 1) } }), Message_TextStartMain.GetElemNewId(5), Message_TextStartMain.GetElemNewId(4)),
            new SearchBotModel(new ModelSearch(new ModelWord[][] { new ModelWord[] { new ModelWord("аудитория", 1), new ModelWord("кабинет", 1) } }), Message_TextStartMain.GetElemNewId(6), Message_TextStartMain.GetElemNewId(7))
        };

        static MainPage() => myStemProcess.GetText("Привет, мир");

        private KitButton buttonsQuestion;
        public string searchStr;
        public byte state = 0;
        public Text answer;

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage) => ResetLastMessenge(inBot);
        public override void EventStoreLoad(ObjectDataMessageInBot inBot, bool state) => LoadButtons(inBot);

        public override void EventInMessageNOVGU(ObjectDataMessageInBot inBot)
        {
            switch (state)
            {
                case 0:
                    searchStr = inBot.MessageText;
                    LoadButtons(inBot);
                    state = 1;
                    ResetLastMessenge(inBot);
                    break;
                case 1:
                    if (!buttonsQuestion.CommandInvoke(inBot))
                        ResetLastMessenge(inBot);
                    break;
                case 2:
                    if (!BackState(inBot))
                        ResetLastMessenge(inBot);
                    break;
            }
        }
        private void LoadButtons(ObjectDataMessageInBot inBot) => buttonsQuestion = KitButton.GenerateKitButtonsTexts(GetNamesButton(inBot).Select(x => new string[] { x }).ToArray(), CommandInvoke, 1d);
        private string[] GetNamesButton(ObjectDataMessageInBot inBot) => searchBotModels.Select(x => (x.question, x.modelSearch.GetDegreeSimilarity(myStemProcess.GetWords(searchStr)))).OrderByDescending(x => x.Item2).Where(x => x.Item2 >= 0.5f).Select(x => $"{x.question.GetText(inBot)} {(uint)(x.Item2 * 100)}%").Take(10).Append(Button_TextBack.GetText(inBot)).ToArray();
        private void CommandInvoke(ObjectDataMessageInBot inBot, string text, object data)
        {
            if (!BackState(inBot))
            {
                if (text.Contains('%'))
                    text = text.Substring(0, text.LastIndexOf(' '));
                foreach (var elem in searchBotModels)
                {
                    if (elem.question.GetText(inBot) == text)
                    {
                        answer = elem.text;
                        state = 2;
                        ResetLastMessenge(inBot);
                        return;
                    }
                }
            }
        }
        private bool BackState(ObjectDataMessageInBot inBot)
        {
            if ((inBot.CallbackData ?? inBot.MessageText).ToLower() == Button_TextBack.GetText(inBot).ToLower())
            {
                state = 0;
                ResetLastMessenge(inBot);
                return true;
            }
            return false;
        }
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot)
        {
            switch (state)
            {
                case 0:
                    SendDataBot(new ObjectDataMessageSend(inBot) { Text = string.Format(Message_TextStartMain.GetText(inBot), searchBotModels.Length), media = new ObjectDataMessageSend.Media[] { new ObjectDataMessageSend.Media("https://im0-tub-ru.yandex.net/i?id=0afb996b2e497cddd00edba1d00759be&n=13", ObjectDataMessageSend.Media.MediaType.Photo) } });
                    break;
                case 1:
                    SendDataBot(new ObjectDataMessageSend(inBot) { ButtonsMessage = buttonsQuestion, TextObj = Message_TextQuestionSelect });
                    break;
                case 2:
                    SendDataBot(new ObjectDataMessageSend(inBot) { ButtonsMessage = new Button[][] { new Button[] { new Button(Button_TextBack, (inBot, s, data) => { return true; }) } }, TextObj = answer });
                    break;
            }
        }
        private class SearchBotModel
        {
            public ModelSearch modelSearch { get; init; }
            public Text text { get; init; }
            public Text question { get; init; }

            public SearchBotModel(ModelSearch modelSearch, Text text, Text question) => (this.modelSearch, this.text, this.question) = (modelSearch, text, question);
        }
    }
}
