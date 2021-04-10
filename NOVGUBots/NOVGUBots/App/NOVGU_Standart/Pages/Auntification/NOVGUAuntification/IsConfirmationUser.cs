using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.Bots.Model.Buttons;
using System;
using BotsCore.Moduls.Translate;
using Newtonsoft.Json.Linq;
using BotsCore;
using System.Linq;
using BotsCore.User;
using static NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification.BindingNOVGU;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification
{
    public class IsConfirmationUser : Page
    {
        public const string NamePage = "NovguUser=Регистрация->Подтверждение?";
        private static readonly ModelMarkerTextData Message_TextStart = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 36);
        private static readonly KitButton MessageButtons = new(new Button[][]
            {
                new Button[]
                {
                    new Button(Message_TextStart.GetElemNewId(37), (inBot, s, data) => { ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, UserRegister.GetUserState(inBot) == UserRegister.UserState.Student ? Student.User.NamePage : ConfirmationUser.NamePage, data); return true; }),
                },
                new Button[]
                {
                    new Button(Message_TextStart.GetElemNewId(38), (inBot, s, data) =>
                    {
                        if(UserRegister.GetInfoRegisterUser(inBot).HasFlag(UserRegister.RegisterState.NewUser))
                        {
                            ManagerPage.ClearHistoryPage(inBot);
                            UserRegister.RemoveFlag(UserRegister.RegisterState.NewUser, inBot);
                            ManagerPage.ResetSendKeyboard(inBot);
                            ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_Main);
                        }
                        else
                            ManagerPage.SetBackPage(inBot);
                        return true;
                    }),
                }
            });

        public RegisterInfo registerInfo;

        public override void EventOpen(ObjectDataMessageInBot inBot, Type oldPage, object dataOpenPage)
        {
            registerInfo = RegisterInfo.Load(dataOpenPage);
            ResetLastMessenge(inBot);
        }
        public override void EventInMessage(ObjectDataMessageInBot inBot)
        {
            if (!MessageButtons.CommandInvoke(inBot, registerInfo))
                ResetLastMessenge(inBot);
        }
        public override void ResetLastMessenge(ObjectDataMessageInBot inBot) => SendDataBot(new ObjectDataMessageSend(inBot) { Text = Student.Main.GetTextMainFormat(registerInfo.textsHistory, Message_TextStart.GetText(inBot), inBot), ButtonsMessage = MessageButtons });
        public static void SetNextPageStudentOrTeacer(ObjectDataMessageInBot inBot, uint countClearPage, object dopData)
        {
            RegisterInfo registerInfo = RegisterInfo.Load(dopData);
            UserRegister.SetRegisterInfo(registerInfo, inBot);
            UserRegister.AddFlag(UserRegister.RegisterState.GroupOrTeacherSet, inBot);
            UserRegister.RegisterState info = UserRegister.GetInfoRegisterUser(inBot);
            if (info.HasFlag(UserRegister.RegisterState.LoginPasswordSet))
            {
                ManagerPage.ClearHistoryListPage(inBot, countClearPage + 1);
                ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, NamePage, registerInfo);
            }
            else if (info.HasFlag(UserRegister.RegisterState.NewUser))
            {
                UserRegister.RemoveFlag(UserRegister.RegisterState.NewUser, inBot);
                ManagerPage.ClearHistoryPage(inBot);
                ManagerPage.ResetSendKeyboard(inBot);
                ManagerPage.SetPageSaveHistory(inBot, CreatePageAppStandart.NameApp, CreatePageAppStandart.NamePage_Main);
            }
            else
            {
                ManagerPage.ClearHistoryListPage(inBot, countClearPage);
                ManagerPage.SetBackPage(inBot);
            }
        }
        public static Moduls.NOVGU_SiteData.Model.User[] FilterUsers(Moduls.NOVGU_SiteData.Model.User[] users) => users?.Where(x => (!string.IsNullOrWhiteSpace(x.Email)) && ManagerUser.SearchUser((u) => (UserRegister.GetUser(u) == x.IdString && UserRegister.GetInfoRegisterUser(u).HasFlag(UserRegister.RegisterState.ConnectNovgu))) == null)?.ToArray();
    }
}
