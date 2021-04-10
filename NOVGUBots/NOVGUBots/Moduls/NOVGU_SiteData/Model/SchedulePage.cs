using BotsCore.Moduls;
using BotsCore.Moduls.Translate;
using Newtonsoft.Json;
using NOVGUBots.Moduls.NOVGU_SiteData.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;

namespace NOVGUBots.Moduls.NOVGU_SiteData.Model
{
    public class SchedulePage
    {
        public delegate void Update(List<object> updateInfo, object oldData, object newData);

        [JsonProperty]
        public TypePars TypeInstitute { get; private set; }
        [JsonProperty]
        public Text Name { get; private set; }
        [JsonProperty]
        public string Url { get; private set; }
        [JsonIgnore]
        public Update EventUpdateInstitute { get; set; }
        [JsonProperty]
        public InstituteCollege[] Institute { get; private set; } = Array.Empty<InstituteCollege>();

        public SchedulePage(Text name, string urlPage, TypePars typeInstitute) => (Name, Url, TypeInstitute) = (name, urlPage, typeInstitute);
        public void UpdateData(InstituteCollege[] newData, Lang.LangTypes[] langs)
        {
            List<object> listUpdate = null;
            if (Institute.Length > 0)
            {
                bool isUpdate = false;
                IEnumerable<object> newDataSet;
                (isUpdate, newDataSet) = IUpdated.Update(newData, Institute, ref listUpdate);
                if (isUpdate)
                    SetDataInstitute(newDataSet.Select(x => (InstituteCollege)x).ToArray());
            }
            else
                SetDataInstitute(newData);
            void SetDataInstitute(InstituteCollege[] instituteColleges)
            {
                if (langs != null && instituteColleges != null)
                {
                    instituteColleges.AsParallel().ForAll(x => ((ITranslatable)x).Translate(langs));
                    foreach (var lang in langs)
                        Name.GetText(lang);
                }
                if (EventUpdateInstitute != null && listUpdate != null)
                {
                    try
                    {
                        EventUpdateInstitute.Invoke(listUpdate, Institute, instituteColleges);
                    }
                    catch (Exception e)
                    {
                        EchoLog.Print($"Произошла ошибка при обработки события обновления {Name}: {e.Message}");
                    }
                }
                lock (Institute)
                {
                    Institute = instituteColleges;
                }
            }
        }
    }
}
