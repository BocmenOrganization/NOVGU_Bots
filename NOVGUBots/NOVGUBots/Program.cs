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
using System.Text;
using System.Net.Http;
using System.IO;
using System;
using Newtonsoft.Json.Linq;

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
            new CreatePageAppStandart(),
            new App.Schedule.CretePageSchedule()
        };
        /// <summary>
        /// 
        /// </summary>
        public static void Main()
        {
            ObjectSettingCostum settingData = new(SettingPath);
            // Вычлинение некоторых настроек в поля
            NOVGUSetting.Start(settingData);
            // Запуск парсера
            DataNOVGU.Start();
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
                new ModelTableText(CreatePageAppStandart.NameTableText, NOVGUSetting.langs),
                new ModelTableUniversal<Media[]>(CreatePageAppStandart.NameTableMedia),
                new ModelTableString(CreatePageAppStandart.NameTableString),
                new ModelTableText(App.Schedule.CretePageSchedule.NameTableText),
                new ModelTableUniversal<Media[]>(App.Schedule.CretePageSchedule.NameTableMedia),
                new ModelTableString(App.Schedule.CretePageSchedule.NameTableString)
            };
            standartTables = new ModelUpdateTablesInternet
                (
                NOVGUSetting.objectSetting.GetValue("TablesConnect_HostTablesUrl"),
                string.IsNullOrWhiteSpace(NOVGUSetting.objectSetting.GetValue("TablesConnect_HostUpdateTime")) ? 100 : ulong.Parse(NOVGUSetting.objectSetting.GetValue("TablesConnect_HostUpdateTime")),
                NOVGUSetting.objectSetting.GetValue("TablesConnect_HostPassword"),
                tables
                );
            GlobalTableManager.AddTables(CreatePageAppStandart.NameApp, tables.Take(3).ToArray());
            GlobalTableManager.AddTables(App.Schedule.CretePageSchedule.NameApp, tables.Skip(3).ToArray());
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
            var dataMaling = JsonConvert.DeserializeObject<(string adressSMPT, int port, (string email, string password)[])>(NOVGUSetting.objectSetting.GetValue("AuthorizationMailing"));
            AuthorizationMailing.Start(dataMaling.adressSMPT, dataMaling.port, dataMaling.Item3);
        }
    }
}
