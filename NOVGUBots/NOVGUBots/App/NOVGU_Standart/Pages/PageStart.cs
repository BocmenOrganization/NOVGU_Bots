using BotsCore.Bots;
using BotsCore.Bots.Interface;
using BotsCore.Bots.Model;
using BotsCore.Bots.Model.Buttons;
using System.Threading.Tasks;
using static BotsCore.Bots.Model.ObjectDataMessageSend;

namespace NOVGUBots.App.NOVGU_Standart.Pages
{
    public class PageStart : IPage
    {
        public KitButton GetKeyboardButtons(ObjectDataMessageInBot inBot)
        {
            return null;
        }
        private bool popavs = false;
        private bool df = false;

        public void InMessage(ObjectDataMessageInBot inBot)
        {
            if (df) return;
            if (popavs)
            {
                df = true;
                popavs = false;
                Task.Run(() =>
                {
                    while (df)
                    {
                        ManagerBots.SendDataBot(new ObjectDataMessageSend(inBot)
                        {
                            SaveInfoMessenge = false,
                            Text = $"Ну вот ты и окончательно попался\nТекущие время: {System.DateTime.Now}",
                            ButtonsMessage = new Button[][] { new Button[] { new Button("Не кликай", null) } },
                            media =
                            new Media[]
                            {
                            new Media("https://sun9-30.userapi.com/VXNUMekNDwuDHGIJwxJmi2ANjOP8UorOv56cqw/2sqiS6ksKhA.jpg", Media.MediaType.Photo)
                            }
                        });
                        System.Threading.Thread.Sleep(1000);
                    }
                });
                return;
            }
            popavs = !popavs;
            Task.Run(() =>
            {
                while (popavs)
                {
                    ManagerBots.SendDataBot(new ObjectDataMessageSend(inBot)
                    {
                        SaveInfoMessenge = true,
                        Text = $"Текущие время: {System.DateTime.Now}",
                        ButtonsMessage = new Button[][] { new Button[] { new Button("Не кликай", null) } },
                        //   ButtonsKeyboard = new Button[][] { new Button[] { new Button("Клава", null) } },
                        media =
                            new Media[]
                            {
                            new Media("https://im0-tub-ru.yandex.net/i?id=2ad01659efb38de6111bba2e26f5d888&n=13", Media.MediaType.Photo)
                            }
                    });
                    System.Threading.Thread.Sleep(1000);
                }
            });
        }

        public void LoadPageStore(ObjectDataMessageInBot inBot)
        {

        }

        public void ResetLastMessenge(ObjectDataMessageInBot inBot)
        {

        }

        public void SetPage(ObjectDataMessageInBot inBot)
        {

        }
    }
}
