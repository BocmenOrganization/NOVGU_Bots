using BotsCore.Bots.Interface;
using BotsCore.Bots.Model;
using BotsCore.Bots.Model.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOVGUBots.App.NOVGU_Standart.Pages
{
    public partial class AppNovgu
    {
        public class PageSetting : IPage
        {
            public KitButton GetKeyboardButtons(ObjectDataMessageInBot inBot)
            {
                throw new NotImplementedException();
            }

            public void InMessage(ObjectDataMessageInBot inBot)
            {
                throw new NotImplementedException();
            }

            public void LoadPageStore(ObjectDataMessageInBot inBot)
            {
                throw new NotImplementedException();
            }

            public void ResetLastMessenge(ObjectDataMessageInBot inBot)
            {
                throw new NotImplementedException();
            }

            public void SetPage(ObjectDataMessageInBot inBot)
            {
                throw new NotImplementedException();
            }
        }
    }
}
