using HtmlAgilityPack;
using NOVGUBots.Moduls.NOVGU_SiteData.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace NOVGUBots.Moduls.NOVGU_SiteData
{
    public static class Parser
    {
        public const string Host = "https://www.novsu.ru";

        public static InstituteCollege[] ParsInstitute(string Html, TypePars typePars, ParalelSetting paralelSetting = ParalelSetting.None)
        {
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(Html);
            HtmlNodeCollection nodesInstitute;
            InstituteCollege[] institutes = null;
            if (typePars == TypePars.InstituteFullTime || typePars == TypePars.InstituteInAbsentia)
            {
                nodesInstitute = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class, 'block_content content')]//table[contains(@class, 'viewtable')]");
                if (nodesInstitute != default)
                {
                    var dopData = htmlDocument.DocumentNode.SelectNodes("//div[@class='col_element'][2]//div[@class='block2']")?.Select(x => (ClearText(x.SelectSingleNode(x.XPath + "/h2")?.InnerText), x.SelectSingleNode(x.XPath + "/table[@class='viewtable']")?.OuterHtml))?.ToArray();
                    List<(string name, string html, string dopHtml)> dataPareseInstitute = new();

                    for (int i = 0; (i + 4) <= nodesInstitute.Count; i += 4)
                    {
                        string name = ClearText(nodesInstitute[i].InnerText);
                        dataPareseInstitute.Add
                            ((
                                name,
                                nodesInstitute[i + 2].InnerHtml,
                                dopData?.FirstOrDefault(x => x.Item1 == name).OuterHtml
                            ));
                    }

                    if ((paralelSetting & ParalelSetting.Institute) > 0)
                        institutes = dataPareseInstitute.AsParallel().Select(x => new InstituteCollege(x.name, x.html, x.dopHtml, typePars, paralelSetting)).ToArray();
                    else
                        institutes = dataPareseInstitute.Select(x => new InstituteCollege(x.name, x.html, x.dopHtml, typePars, paralelSetting)).ToArray();
                }
            }
            else
            {
                nodesInstitute = htmlDocument.DocumentNode.SelectNodes("//div[@class='row_el npe_centerbigcolumn']//div[@class='col_element']");

                if (nodesInstitute != default)
                {
                    if ((paralelSetting & ParalelSetting.Institute) > 0)
                        institutes = nodesInstitute.AsParallel().Select(x => new InstituteCollege(null, x.InnerHtml, null, typePars, paralelSetting)).ToArray();
                    else
                        institutes = nodesInstitute.Select(x => new InstituteCollege(null, x.InnerHtml, null, typePars, paralelSetting)).ToArray();
                }
            }
            institutes = institutes?.Where(x => x?.Courses != null).ToArray();
            return institutes;
        }
        public static UserTeacher[] GetTeachers(string Html, ParalelSetting paralelSetting = ParalelSetting.None)
        {
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(Html);
            IEnumerable<(string url, string name)> UrlTeachers = htmlDocument.DocumentNode.SelectNodes("//div[@id='npe_instance_1103357_npe_content']/table[@class='viewtable']//a[@href]")?.Select(x => (x.Attributes["href"].Value, ClearText(x.InnerText))).Where(x => !string.IsNullOrEmpty(x.Value) && x.Item2?.Length > 3);
            htmlDocument = null;
            if (paralelSetting.HasFlag(ParalelSetting.PeopleTeacher))
                return UrlTeachers?.AsParallel().Select(x => new UserTeacher(x.url, x.name)).ToArray();
            else
                return UrlTeachers?.Select(x => new UserTeacher(x.url, x.name)).ToArray();
        }
        public static DateTime[][][] GetCalendar(string Html)
        {
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(Html);
            HtmlNodeCollection htmlNodes = htmlDocument.DocumentNode.SelectNodes("//div[@id='npe_instance_1103492_npe_content']/div[@class='block_3padding']/table//td");
            uint @switch = 0;
            return htmlNodes?.Where((x, i) => i % 2 != 0).Select(x => x.InnerText.Split("-")?.Select(x => DateTime.Parse(x)).ToArray()).GroupBy(x => ++@switch % 2 == 0).Select(x => x?.ToArray()).ToArray();
        }
        public static string ClearText(string text) => string.Join(" ", Regex.Replace(text, @"[\r\n\t]", "")?.Split(' ')?.Where(x => !string.IsNullOrWhiteSpace(x)));
        [Flags]
        public enum ParalelSetting : uint
        {
            None = 0,
            Institute = 1,
            Course = 2,
            Group = 4,
            PeopleGroup = 8,
            PeopleTeacher = 16,
            Schedule = 32
        }
        public enum TypePars : uint
        {
            /// <summary>
            /// Парсинг очников (институт)
            /// </summary>
            InstituteFullTime = 0,
            /// <summary>
            /// Парсинг заочников (институт)
            /// </summary>
            InstituteInAbsentia = 1,
            /// <summary>
            /// Парсинг колледжей
            /// </summary>
            College = 2,
            /// <summary>
            /// Парсинг сессии
            /// </summary>
            Session = 3,
            /// <summary>
            /// Парсинг преподов
            /// </summary>
            Teacher = 4
        }
    }
}