﻿using HtmlAgilityPack;
using NOVGUBots.Moduls.NOVGU_SiteData.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using static NOVGUBots.Moduls.NOVGU_SiteData.Model.InstituteCollege;
using Newtonsoft.Json;

namespace NOVGUBots.Moduls.NOVGU_SiteData
{
    public static class Parser
    {
        public const string Host = "https://www.novsu.ru";

        public static void Start()
        {
            var r = ParsInstitute(new WebClient().DownloadString("https://www.novsu.ru/univer/timetable/zaochn/"), TypePars.InstituteInAbsentia, ParalelSetting.Group | ParalelSetting.PeopleGroup | ParalelSetting.Course).First();
        }
        public static InstituteCollege[] ParsInstitute(string Html, TypePars typePars, ParalelSetting paralelSetting = ParalelSetting.None)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(Html);
            HtmlNodeCollection nodesInstitute;
            InstituteCollege[] institutes = null;
            if (typePars == TypePars.InstituteFullTime || typePars == TypePars.InstituteInAbsentia)
            {
                nodesInstitute = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class, 'block_content content')]//table[contains(@class, 'viewtable')]");
                if (nodesInstitute != default)
                {
                    var dopData = htmlDocument.DocumentNode.SelectNodes("//div[@class='col_element'][2]//div[@class='block2']")?.Select(x => (ClearText(x.SelectSingleNode(x.XPath + "/h2")?.InnerText), x.SelectSingleNode(x.XPath + "/table[@class='viewtable']")?.OuterHtml))?.ToArray();
                    List<(string name, string html, string dopHtml)> dataPareseInstitute = new List<(string name, string html, string dopHtml)>();

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
            institutes = institutes?.Where(x => x?.courses != null).ToArray();
            File.WriteAllText("er.txt", JsonConvert.SerializeObject(institutes?.ToArray(), Formatting.Indented));
            return institutes;
        }
        public static string ClearText(string text) => string.Join(" ", Regex.Replace(text, @"[\r\n\t]", "").Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)));
        public enum ParalelSetting
        {
            None = 0,
            Institute = 1,
            Course = 2,
            Group = 4,
            PeopleGroup = 8,
            PeopleTeacher = 16
        }
    }
}