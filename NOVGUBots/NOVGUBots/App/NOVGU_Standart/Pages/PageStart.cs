using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using BotsCore;
using static BotsCore.Bots.Model.ObjectDataMessageSend;
using System.Threading.Tasks;

namespace NOVGUBots.App.NOVGU_Standart.Pages
{
    public class PageStart : Page
    {
        private static readonly ModelMarkerUneversalData<Media[]> Message_MedaiGroup = new(CreatePageAppStandart.NameApp, "MainMediaNOVGU", 0);
        private static readonly ModelMarkerUneversalData<Media[]> Message_MedaiMessage = Message_MedaiGroup.GetElemNewId(1);
        private static readonly ModelMarkerTextData Message_TextStart = new(CreatePageAppStandart.NameApp, "MainTextNOVGU", 1);
        private static readonly ModelMarkerTextData Buttons_Text = Message_TextStart.GetElemNewId(2);
        private static readonly KitButton Keyboard_Further = new(new Button[][] { new Button[] { new Button(Buttons_Text, (inBot, data) => { ManagerPage.SetPage(inBot, CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_RegisterMain); }) } });
        public override void EventInMessage(ObjectDataMessageInBot inBot) => StartMessage(inBot);
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => StartMessage(inBot);
        public override KitButton GetKeyboardButtons(ObjectDataMessageInBot inBot) => Keyboard_Further;
        private void StartMessage(ObjectDataMessageInBot inBot)
        {
            Task.Run(() =>
            {
                SendDataBot(new ObjectDataMessageSend(inBot) { media = Message_MedaiGroup, IsSaveInfoMessenge = false, ButtonsKeyboard = Keyboard_Further.GetButtons() }).Wait();
                SendDataBot(new ObjectDataMessageSend(inBot) { Text = Message_TextStart.GetText(inBot), media = Message_MedaiMessage, IsSaveInfoMessenge = false }).Wait();
            });
        }
    }
}