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
using NOVGUBots.App.NOVGU_Standart.Pages;
using NOVGUBots.Moduls;
using NOVGUBots.Moduls.NOVGU_SiteData;
using NOVGUBots.SettingCore;

namespace NOVGUBots
{
    static class Program
    {
        private const string SettingPath = @"Settings\GlobalSetting.txt";
        private static ModelUpdateTablesInternet standartTables;
        private static readonly object[] apps = new object[]
        {
            new CreatePageAppStandart()
        };
        public static void Main()
        {
            Parser.Start();
            return;
            // Получение главного файла настроек
            ObjectSettingCostum settingData = new ObjectSettingCostum(SettingPath);
            // Вычлинение некоторых настроек в поля
            NOVGUSetting.Start(settingData);
            // Применение настроек для менеджера страниц
            ManagerPage.Start(new SettingManagerPage(NOVGUSetting.objectSetting));
            // Инцилизация базы пользователей
            ManagerUser.Start(new ObjectSettingCostum(NOVGUSetting.objectSetting.GetValue("ManagerUser_PathFileSetting")));
            // Загрузка базовых таблиц
            LoadStandartTables();
            // Подгрузка пвсех приложений
            LoadApp();
            // Инцилизация ботов
            LoadBots();
            // Вечное ожидание чтоб программа не закрылась
            System.Diagnostics.Process.GetCurrentProcess().WaitForExit();
        }
        public static void LoadApp()
        {
            foreach (var app in apps)
                ManagerPageNOVGU.AddApp(app);
        }
        private static void LoadStandartTables()
        {
            (string type, string name)[] tablesInfo = JsonConvert.DeserializeObject<(string type, string name)[]>(NOVGUSetting.objectSetting.GetValue("TablesConnect_TablesListInfo"));
            ITable[] tables = new ITable[tablesInfo.Length];
            for (int i = 0; i < tablesInfo.Length; i++)
            {
                switch (tablesInfo[i].type)
                {
                    case "String":
                        tables[i] = new ModelTableString(tablesInfo[i].name);
                        break;
                    case "Text":
                        tables[i] = new ModelTableText(tablesInfo[i].name);
                        break;
                    case "Object":
                        tables[i] = new ModelTableUniversal<object>(tablesInfo[i].name);
                        break;
                }
            }
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
            ManagerBots.StartBots();
        }
    }
}
