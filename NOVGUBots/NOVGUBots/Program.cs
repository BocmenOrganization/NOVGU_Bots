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
using NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification;
using System.Linq;
using NOVGUBots.App.Schedule.Pages;
using BotsCore.Moduls;

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
        public static void Main()
        {
            EchoLog.Print("\r\n          ██████╗░░█████╗░░█████╗░███╗░░░███╗███████╗███╗░░██╗  ░█████╗░░█████╗░██████╗░██████╗░\r\n          ██╔══██╗██╔══██╗██╔══██╗████╗░████║██╔════╝████╗░██║  ██╔══██╗██╔══██╗██╔══██╗██╔══██╗\r\n          ██████╦╝██║░░██║██║░░╚═╝██╔████╔██║█████╗░░██╔██╗██║  ██║░░╚═╝██║░░██║██████╔╝██████╔╝\r\n          ██╔══██╗██║░░██║██║░░██╗██║╚██╔╝██║██╔══╝░░██║╚████║  ██║░░██╗██║░░██║██╔══██╗██╔═══╝░\r\n          ██╔══██╗██║░░██║██║░░██╗██║╚██╔╝██║██╔══╝░░██║╚████║  ██║░░██╗██║░░██║██╔══██╗██╔═══╝░\r\n          ██████╦╝╚█████╔╝╚█████╔╝██║░╚═╝░██║███████╗██║░╚███║  ╚█████╔╝╚█████╔╝██║░░██║██║░░░░░\r\n          ╚═════╝░░╚════╝░░╚════╝░╚═╝░░░░░╚═╝╚══════╝╚═╝░░╚══╝  ░╚════╝░░╚════╝░╚═╝░░╚═╝╚═╝░░░░░\r\n\r\n");
            ObjectSettingCostum settingData = new(SettingPath);
            EchoLog.Print("Настройки были полученны");
            // Вычлинение некоторых настроек в поля
            NOVGUSetting.Start(settingData);
            EchoLog.Print("Настройки были применены");
            // Инцилизация базы пользователей
            ManagerUser.Start(new ObjectSettingCostum(NOVGUSetting.objectSetting.GetValue("ManagerUser_PathFileSetting")));
            EchoLog.Print("Данные пользователей были подгруженны");
            // Запуск парсера
            EchoLog.Print("Запускаю парсер");
            StatickCheckUpdate.Start();
            EchoLog.Print("Парсер был запущен");
            EchoLog.Print("Были подключены события обновления расписания");
            // Загрузка базовых таблиц
            EchoLog.Print("Получаю данные таблиц");
            LoadStandartTables();
            EchoLog.Print("Данные таблиц были полученны");
            // Подгрузка всех приложений
            EchoLog.Print("Подгружаю подключенные страницы");
            LoadApp();
            EchoLog.Print("Страницы были загруженны");
            // Загруза почтового сервиса подтверждения авторизации
            LoadMaling();
            EchoLog.Print("Почтовый сервис был запущен");
            // Инцилизация ботов
            EchoLog.Print("Запускаю клиенты ботов");
            LoadBots();
            EchoLog.Print("Боты были запущенны");
            // Применение настроек для менеджера страниц
            ManagerPage.Start(new SettingManagerPage());
            EchoLog.Print("Данные менеджера страниц были установленны");
            // Старт постоянного поиска обновлений в расписании
            DataNOVGU.Start();
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
