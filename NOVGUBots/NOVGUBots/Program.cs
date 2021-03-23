/*
 * Projet code: https://github.com/BocmenOrganization
 * Feedback: https://vk.com/denisivanov220
 * 

            ██████╗░░█████╗░░█████╗░███╗░░░███╗███████╗███╗░░██╗  ░█████╗░░█████╗░██████╗░██████╗░
 [@@]       ██╔══██╗██╔══██╗██╔══██╗████╗░████║██╔════╝████╗░██║  ██╔══██╗██╔══██╗██╔══██╗██╔══██╗
\ °° /      ██████╦╝██║░░██║██║░░╚═╝██╔████╔██║█████╗░░██╔██╗██║  ██║░░╚═╝██║░░██║██████╔╝██████╔╝
 [  ]       ██╔══██╗██║░░██║██║░░██╗██║╚██╔╝██║██╔══╝░░██║╚████║  ██║░░██╗██║░░██║██╔══██╗██╔═══╝░
  ||        ██╔══██╗██║░░██║██║░░██╗██║╚██╔╝██║██╔══╝░░██║╚████║  ██║░░██╗██║░░██║██╔══██╗██╔═══╝░
 _||_       ██████╦╝╚█████╔╝╚█████╔╝██║░╚═╝░██║███████╗██║░╚███║  ╚█████╔╝╚█████╔╝██║░░██║██║░░░░░
            ╚═════╝░░╚════╝░░╚════╝░╚═╝░░░░░╚═╝╚══════╝╚═╝░░╚══╝  ░╚════╝░░╚════╝░╚═╝░░╚═╝╚═╝░░░░░
*/

using BotsCore;
using BotsCore.Bots;
using BotsCore.Bots.BotsModel;
using BotsCore.Moduls.GetSetting;
using BotsCore.Moduls.Tables.Interface;
using BotsCore.Moduls.Tables.ModelTables;
using BotsCore.Moduls.Tables.Services;
using BotsCore.User;
using Newtonsoft.Json;
using NOVGUBots.Moduls;
using NOVGUBots.SettingCore;
using static BotsCore.Bots.Model.ObjectDataMessageSend;
using NOVGUBots.App.NOVGU_Standart;
using NOVGUBots.Moduls.NOVGU_SiteData;
using System.Net.Mail;
using NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification;
using System.Linq;
using System.Net;

namespace NOVGUBots
{
    static class Program
    {
        private const string SettingPath = @"Settings\GlobalSetting.txt";
#pragma warning disable IDE0052 // Удалить непрочитанные закрытые члены
        private static ModelUpdateTablesInternet standartTables;
#pragma warning restore IDE0052 // Удалить непрочитанные закрытые члены
        private static readonly object[] apps = new object[]
        {
            new CreatePageAppStandart()
        };
        /// <summary>
        /// 
        /// </summary>
        public static void Main()
        {
            //string s = string.Join(",\n", Lang.LangNameRu.Select((x, i) => $"'{x}': {i}"));
            //return;
            // DataNOVGU.LoadNewData();
            //var r = JsonConvert.DeserializeObject<UserTeacher[]>(File.ReadAllText("teacher.json"));//Parser.GetTeachers(new WebClient().DownloadString("https://www.novsu.ru/univer/timetable/ochn/i.1103357/?page=allTeachersTimetable"), Parser.ParalelSetting.PeopleTeacher);
            //List<object> list = new List<object>();
            //var rn = JsonConvert.DeserializeObject<UserTeacher[]>(File.ReadAllText("teacher1.json"));
            //var up = IUpdated.Update(r, rn, ref list);
            DataNOVGU.Start();
            // Получение главного файла настроек
            ObjectSettingCostum settingData = new(SettingPath);
            // Вычлинение некоторых настроек в поля
            NOVGUSetting.Start(settingData);
            // Инцилизация базы пользователей
            ManagerUser.Start(new ObjectSettingCostum(NOVGUSetting.objectSetting.GetValue("ManagerUser_PathFileSetting")));
            // Загрузка базовых таблиц
            LoadStandartTables();
            // Загруза почтового сервиса подтверждения авторизации
            LoadMaling();
            // Подгрузка всех приложений
            LoadApp();
            // Инцилизация ботов
            LoadBots();
            // Применение настроек для менеджера страниц
            ManagerPage.Start(new SettingManagerPage());
            // Вечное ожидание чтоб программа не закрылась
            System.Diagnostics.Process.GetCurrentProcess().WaitForExit();
        }
        public static void LoadApp()
        {
            foreach (var app in apps)
                ManagerPageNOVGU.ManagerPageNOVGU.AddApp(app);
        }
        private static void LoadStandartTables()
        {
            ITable[] tables = new ITable[]
            {
                new ModelTableText("MainTextNOVGU", NOVGUSetting.langs),
                new ModelTableUniversal<Media[]>("MainMediaNOVGU"),
                new ModelTableString("MainStringNOVGU")
            };
            standartTables = new ModelUpdateTablesInternet
                (
                NOVGUSetting.objectSetting.GetValue("TablesConnect_HostTablesUrl"),
                string.IsNullOrWhiteSpace(NOVGUSetting.objectSetting.GetValue("TablesConnect_HostUpdateTime")) ? 100 : ulong.Parse(NOVGUSetting.objectSetting.GetValue("TablesConnect_HostUpdateTime")),
                NOVGUSetting.objectSetting.GetValue("TablesConnect_HostPassword"),
                tables
                );
            GlobalTableManager.AddTables(CreatePageAppStandart.NameApp, tables);
        }
        private static void LoadBots()
        {
            (string name, string data)[] botsInfo = JsonConvert.DeserializeObject<(string name, string type)[]>(NOVGUSetting.objectSetting.GetValue("BotsTelegram_ListBots"));
            foreach (var (name, data) in botsInfo)
            {
                switch (name)
                {
                    case "Telegram":
                        ManagerBots.AddBot(new TelegramBot(data, null));
                        break;
                }
            }
        }
        private static void LoadMaling()
        {
            var dataMaling = JsonConvert.DeserializeObject<(string adressSMPT, int port, (string user, string password)[])>(NOVGUSetting.objectSetting.GetValue("AuthorizationMailing"));
            AuthorizationMailing.Start(dataMaling.Item3.Select(x =>
            {
                SmtpClient smtpClient = new SmtpClient(dataMaling.adressSMPT, dataMaling.port);
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(x.user, x.password);
                return (smtpClient, new MailAddress(x.user));
            }).ToArray());
        }
    }
}
