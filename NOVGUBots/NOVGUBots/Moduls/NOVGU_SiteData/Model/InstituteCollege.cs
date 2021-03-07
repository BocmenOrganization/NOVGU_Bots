using BotsCore.Moduls.Translate;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using NOVGUBots.Moduls.NOVGU_SiteData.Interface;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;

namespace NOVGUBots.Moduls.NOVGU_SiteData.Model
{
    public class InstituteCollege : IUpdated
    {
        public string Id;
        public Text Name;
        public Course[] courses;
        public TypePars typePars;
        public InstituteCollege(string name, string html, string dopHtml, TypePars typePars, ParalelSetting paralelSetting)
        {
            if (typePars != TypePars.College)
                Name = new Text(Lang.LangTypes.ru, name);
            this.typePars = typePars;
            var data = LoadData(html, dopHtml, typePars, paralelSetting);
            if (data != null)
            {
                if (name == null && data.Value.id != null)
                    Name = new Text(Lang.LangTypes.ru, data.Value.id);
                Id = data.Value.id;
                courses = data.Value.courses?.Where(x => x?.groups != null).ToArray();
                if (courses != null && courses.Length == 0)
                    courses = null;
            }
        }
        private static (string id, Course[] courses)? LoadData(string html, string dopHtml, TypePars typePars, ParalelSetting paralelSetting)
        {
            (string idInstitute, (string course, (string nameGroup, string UrlSchedule)[])[])? data;
            Course[] courses;
            if (typePars == TypePars.InstituteFullTime || typePars == TypePars.InstituteInAbsentia || typePars == TypePars.Session)
                data = ParsInstitute(html, dopHtml);
            else
                data = ParseCollege(html);

            if (data != null)
            {
                if ((paralelSetting & ParalelSetting.Institute) > 0)
                    courses = data.Value.Item2.Where(x => x.course != default).AsParallel().Select((x, i) => new Course(typePars, paralelSetting, x.course, x.Item2)).ToArray();
                else
                    courses = data.Value.Item2.Where(x => x.course != default).Select((x, i) => new Course(typePars, paralelSetting, x.course, x.Item2)).ToArray();
                return (data.Value.idInstitute, courses);
            }
            return null;
        }
        private static (string idInstitute, (string course, (string nameGroup, string UrlSchedule)[])[])? ParsInstitute(string html, string dopHtml)
        {
            if (html == default)
                return default;
            int id = -1;
            HtmlDocument htmlDocumentCourse = new HtmlDocument();
            htmlDocumentCourse.LoadHtml(html);
            HtmlNodeCollection nodesCourse = htmlDocumentCourse.DocumentNode.SelectNodes("//tr[2]//td");
            (string course, List<(string nameGroup, string UrlSchedule)>)[] dataResul = ParseCollege(dopHtml)?.Item2?.Select(x => (x.course, x.Item2?.ToList())).ToArray();
            (string course, List<(string nameGroup, string UrlSchedule)>)[] dataMain = LaodDataInstitute(htmlDocumentCourse.DocumentNode.SelectNodes("//tr[2]//td"));
            (string course, List<(string nameGroup, string UrlSchedule)>)[] dataBasement = LaodDataInstitute(htmlDocumentCourse.DocumentNode.SelectNodes("//tr[3]//td"));

            if (dataResul != null && dataMain != null)
                AddElems(ref dataResul, ref dataMain);
            else if (dataResul == null)
            {
                dataResul = dataMain;
                dataMain = null;
            }

            if (dataBasement != null && dataResul != null)
                AddElems(ref dataResul, ref dataBasement);

            static void AddElems(ref (string course, List<(string nameGroup, string UrlSchedule)>)[] value1, ref (string course, List<(string nameGroup, string UrlSchedule)>)[] value2)
            {
                for (int i = 0; i < value2.Length; i++)
                {
                    if (value1[i].Item2 == null)
                    {
                        value1[i].course = value2[i].course;
                        value1[i].Item2 = new List<(string nameGroup, string UrlSchedule)>();
                    }
                    if (value2[i].Item2 != null)
                    {
                        value1[i].Item2.AddRange(value2[i].Item2);
                        value1[i].Item2 = value1[i].Item2.Distinct().ToList();
                    }
                }
            }
            (string course, List<(string nameGroup, string UrlSchedule)>)[] LaodDataInstitute(HtmlNodeCollection nodesCourse)
            {
                if (nodesCourse != default)
                {
                    (string course, (string nameGroup, string UrlSchedule)[])[] data = new (string course, (string nameGroup, string UrlSchedule)[])[nodesCourse.Count];
                    for (int itemCourse = 0; itemCourse < nodesCourse.Count; itemCourse++)
                    {
                        data[itemCourse].course = $"{itemCourse + 1} курс";
                        HtmlDocument htmlDocumentGroup = new HtmlDocument();
                        htmlDocumentGroup.LoadHtml(nodesCourse[itemCourse].InnerHtml);
                        HtmlNodeCollection nodesGroup = htmlDocumentGroup.DocumentNode.SelectNodes("//a");
                        if (nodesGroup != default)
                        {
                            data[itemCourse].Item2 = new (string nameGroup, string UrlSchedule)[nodesGroup.Count];
                            for (int itemGroup = 0; itemGroup < nodesGroup?.Count; itemGroup++)
                            {
                                data[itemCourse].Item2[itemGroup].nameGroup = ClearText(nodesGroup[itemGroup].InnerText);
                                if (nodesGroup[itemGroup].Attributes.Contains("href"))
                                    data[itemCourse].Item2[itemGroup].UrlSchedule = nodesGroup[itemGroup].Attributes["href"].Value;
                                if (id == -1 && nodesGroup[itemGroup].Attributes.Contains("href"))
                                {
                                    string idInstituteS = nodesGroup[itemGroup].Attributes["href"].Value;
                                    idInstituteS = idInstituteS.Remove(0, idInstituteS.IndexOf("instId=") + 7);
                                    idInstituteS = idInstituteS.Substring(0, idInstituteS.IndexOf('&'));
                                    id = int.Parse(idInstituteS);
                                }
                            }
                        }
                    }
                    return data.Select(x => (x.course, x.Item2?.ToList()))?.ToArray();
                }
                return null;
            }
            return (id.ToString(), dataResul.Select(x => (x.course, x.Item2?.ToArray())).ToArray());
        }
        private static (string idInstitute, (string course, (string nameGroup, string UrlSchedule)[])[])? ParseCollege(string html)
        {
            if (html == default)
                return default;
            HtmlDocument htmlDocumentCourse = new HtmlDocument();
            htmlDocumentCourse.LoadHtml(html);
            string[] NameCourses = htmlDocumentCourse.DocumentNode.SelectNodes("//table[@class='viewtable']//tr[1]//th")?.Select(x => x.InnerText)?.ToArray();
            HtmlNodeCollection nodesCourse = htmlDocumentCourse.DocumentNode.SelectNodes("//table[@class='viewtable']//tr[2]//td");
            if (nodesCourse != default && NameCourses != default && NameCourses.Length == nodesCourse.Count)
            {
                (string course, (string nameGroup, string UrlSchedule)[])[] data = new (string course, (string nameGroup, string UrlSchedule)[])[nodesCourse.Count];
                for (int itemCourse = 0; itemCourse < nodesCourse.Count; itemCourse++)
                {
                    HtmlDocument htmlDocumentGroup = new HtmlDocument();
                    htmlDocumentGroup.LoadHtml(nodesCourse[itemCourse].InnerHtml);
                    HtmlNodeCollection nodesGroup = htmlDocumentGroup.DocumentNode.SelectNodes("//a");
                    if (nodesGroup != default)
                    {
                        data[itemCourse].course = NameCourses[itemCourse];
                        data[itemCourse].Item2 = new (string nameGroup, string UrlSchedule)[nodesGroup.Count];
                        for (int itemGroup = 0; itemGroup < nodesGroup?.Count; itemGroup++)
                        {
                            data[itemCourse].Item2[itemGroup].nameGroup = nodesGroup[itemGroup].InnerText;
                            if (nodesGroup[itemGroup].Attributes.Contains("href"))
                                data[itemCourse].Item2[itemGroup].UrlSchedule = nodesGroup[itemGroup].Attributes["href"].Value;
                        }
                    }
                }
                return (htmlDocumentCourse.DocumentNode.SelectSingleNode("//div[@class='portlet_icons']")?.ParentNode?.InnerText, data);
            }
            return null;
        }

        public override string ToString() => (typePars == TypePars.College ? $"id-Название: {Id}" : $"Id: {Id}, Название: {Name}") + $" Тип: {typePars}";

        public string GetId() => Id;
        public IEnumerable<Text> GetTextsTranslate() => new List<Text>() { Name };
        public IEnumerable<object> GetData() => courses;
        public void SetData(IEnumerable<object> newData) => courses = newData.Select(x => (Course)x).ToArray();

        public enum TypePars
        {
            /// <summary>
            /// Парсинг очников (институт)
            /// </summary>
            InstituteFullTime,
            /// <summary>
            /// Парсинг заочников (институт)
            /// </summary>
            InstituteInAbsentia,
            /// <summary>
            /// Парсинг колледжей
            /// </summary>
            College,
            /// <summary>
            /// Парсинг сессии
            /// </summary>
            Session
        }
    }
}