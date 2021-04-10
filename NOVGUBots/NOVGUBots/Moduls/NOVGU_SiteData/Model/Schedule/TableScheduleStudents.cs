using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;
using static NOVGUBots.Moduls.NOVGU_SiteData.Model.Schedule.Hendler;
using NOVGUBots.Moduls.NOVGU_SiteData.Interface;
using System.Collections.Generic;
using BotsCore.Moduls.Translate;
using System.Linq;
using System;

namespace NOVGUBots.Moduls.NOVGU_SiteData.Model.Schedule
{
    public class TableScheduleStudents : IUpdated
    {
        public TypeSchedule typeSchedule;
        public DayStudents[] DataTable;
        public string UrlSchedule;

        public TableScheduleStudents(string url, TypePars typePars)
        {
            UrlSchedule = Host + url;
            typeSchedule = url.Contains("univer") ? TypeSchedule.Text : TypeSchedule.Files;
            if (typeSchedule == TypeSchedule.Text)
                DataTable = GetDayStudents(UrlSchedule, typePars);
        }
        [Newtonsoft.Json.JsonConstructor]
        private TableScheduleStudents() { }

        public IEnumerable<object> GetData() => DataTable;
        public void SetData(IEnumerable<object> newData) => DataTable = newData?.Select(x=>(DayStudents)x).ToArray();
        public List<Text> GetTextsTranslate()
        {
            List<Text> texts = new();
            if (DataTable != null)
                foreach (var item in DataTable)
                    texts.AddRange(item.GetTextsTranslate());
            return texts;
        }
        public bool Similarity(object e)
        {
            if (e is TableScheduleStudents scheduleStudents)
                return scheduleStudents.typeSchedule == typeSchedule && scheduleStudents.UrlSchedule == UrlSchedule;
            return false;
        }

        public enum TypeSchedule
        {
            Text,
            Files
        }
    }
}