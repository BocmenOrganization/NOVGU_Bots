using BotsCore.Moduls.Tables.Interface;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Timers;
using BotsCore.Moduls.Tables.Services.Model;
using System;
using BotsCore.Moduls;

namespace NOVGUBots.Moduls
{
    /// <summary>
    /// Обновление данных таблиц из сети (как настроить сервер обратитесь к разрабу)
    /// </summary>
    public class ModelUpdateTablesInternet
    {
        private readonly string host;
        private readonly ITable[] tables;
        private ModelTableInfo[] infosTable;
        private Timer timerUpdate;
        private string Password;

        public ModelUpdateTablesInternet(string host, ulong timeUpdate, string Password = null, params ITable[] tables)
        {
            if (tables == null || tables.Count() == 0)
                throw new BotsCore.Model.Exception("Не удалось создать обьект обновления страниц из сети из за отсутствия страниц");
            this.host = host;
            this.tables = tables;
            this.Password = Password;
            LoadAllTablesData();
            timerUpdate = new Timer();
            timerUpdate.Interval = (double)timeUpdate;
            timerUpdate.AutoReset = false;
            timerUpdate.Elapsed += (e, c) => { LoadAllTablesData(); timerUpdate.Start(); };
            timerUpdate.Start();
        }
        /// <summary>
        /// Получения состояния таблиц
        /// </summary>
        private ModelTableInfo[] GetTablesInfo()
        {
            try
            {
                return JsonConvert.DeserializeObject<ModelTableInfo[]>(DownloadText($"{host}?typeRequest=GetTables&tablesName={string.Join("|", tables.Select(x => x.GetNameTable()))}"));
            }
            catch
            {
                throw new BotsCore.Model.Exception("Не удалось загрузить ключи таблиц");
            }
        }
        /// <summary>
        /// Получения список таблиц которые необходимо обновить
        /// </summary>
        private List<ITable> GetInfoUpdateTable(ModelTableInfo[] newModelTableInfos)
        {
            if (infosTable == null)
                return tables.ToList();
            if (newModelTableInfos == null)
                throw new BotsCore.Model.Exception("Не удалось узнать список обновляемых таблиц");
            List<ITable> resul = new List<ITable>();
            foreach (var InfoTable in newModelTableInfos)
            {
                var tableInfo = infosTable.FirstOrDefault(x => x.TableName == InfoTable.TableName);
                if (tableInfo.UpdateKey != InfoTable.UpdateKey)
                    resul.Add(tables.First(x => x.GetNameTable() == tableInfo.TableName));
            }
            return resul;
        }
        /// <summary>
        /// Загрузка данных таблицы
        /// </summary>
        private void LoadDataTable(ITable table)
        {
            var dataTable = JsonConvert.DeserializeObject<ModelContainerDataTable[]>(DownloadText($"{host}?typeRequest=GetTableData&tablesName={table.GetNameTable()}"));
            table.Update(dataTable);
        }
        /// <summary>
        /// Загрузка/обновление всех таблиц
        /// </summary>
        private void LoadAllTablesData()
        {
            try
            {
                var InfaTables = GetTablesInfo();
                var listUpdateTable = GetInfoUpdateTable(InfaTables);
                foreach (var table in listUpdateTable)
                {
                    LoadDataTable(table);
                }
                infosTable = InfaTables;
            }
            catch (Exception e)
            {
                EchoLog.Print($"Ошибка загрузки данных таблиц ошибка: {e.Message}", EchoLog.PrivilegeLog.Error);
            }
        }
        /// <summary>
        /// Загрузка текста
        /// </summary>
        private string DownloadText(string Url)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Password", Password);
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:83.0) Gecko/20100101 Firefox/83.0");
                return client.DownloadString(Url);
            }
        }
    }
}