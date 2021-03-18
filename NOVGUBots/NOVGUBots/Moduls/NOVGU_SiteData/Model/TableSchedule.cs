using System;
using System.Collections.Generic;
using System.Linq;
using static NOVGUBots.Moduls.NOVGU_SiteData.Model.InstituteCollege;
using HtmlAgilityPack;
using System.Net;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;
using BotsCore.Moduls.Translate;

namespace NOVGUBots.Moduls.NOVGU_SiteData.Model
{
    public class TableSchedule
    {
        public TypeSchedule typeSchedule;
        public Day[] DataTable;
        public string UrlSchedule;

        public TableSchedule(string url, TypePars typePars)
        {
            UrlSchedule = Host + url;
            typeSchedule = url.Contains("univer") ? TypeSchedule.Text : TypeSchedule.Files;
            if (typeSchedule == TypeSchedule.Text)
                DataTable = ParsInstitute(UrlSchedule, typePars);
        }
        private static Day[] ParsInstitute(string url, TypePars typePars)
        {
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml((new WebClient()).DownloadString(url));
            HtmlNodeCollection htmlNodesTable = htmlDocument.DocumentNode.SelectNodes("//table[@class='shedultable']//tr");
            DateTime[] StartEndTime = GetInstituteTimesPeriod(htmlDocument);
            List<(string, (DateTime[] times, string[] numGroup, Text[] subject, Href[] teacher, Href[] auditorium, Href[] comment)[])> Days = new();
            if (htmlNodesTable != null)
            {
                for (int i = 1; i < htmlNodesTable.Count; i++)
                {
                    var infoNode = htmlDocument.DocumentNode.SelectSingleNode(htmlNodesTable[i].XPath + $"//td[@rowspan]");
                    if (infoNode != null)
                    {
                        int count = int.Parse(infoNode.Attributes["rowspan"].Value) - 1;
                        Days.Add((ClearText(infoNode.InnerText), GetDayInfo(htmlNodesTable.Skip(i + 1).Take(count).Select(x => x.InnerHtml).ToArray())));
                        i += count;
                    }
                }
            }
            static (DateTime[] times, string[] numGroup, Text[] subject, Href[] teacher, Href[] auditorium, Href[] comment)[] GetDayInfo(string[] lines)
            {
                List<(DateTime[] times, string[] numGroup, Text[] subject, Href[] teacher, Href[] auditorium, Href[] comment)> resul = new();
                for (int i = 0; i < lines.Length; i++)
                {
                    int countLine = 0;
                    if (countLine == 0)
                    {
                        HtmlDocument document = new();
                        document.LoadHtml(lines[i]);
                        HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("/td");
                        var nodeTime = nodes.FirstOrDefault(x => x.Attributes.Contains("rowspan"));
                        _ = int.TryParse(nodeTime.Attributes["rowspan"].Value, out countLine);
                        (DateTime[] times, string[] numGroup, Text[] subject, Href[] teacher, Href[] auditorium, Href[] comment) = (null, new string[countLine], new Text[countLine], new Href[countLine], new Href[countLine], new Href[countLine]);
                        byte offset = 0;
                        times = nodeTime.InnerHtml?.Split("<br>")?.Where(x => !string.IsNullOrWhiteSpace(x))?.Select(x => DateTime.Parse(x))?.ToArray();
                    addInfoLine:
                        numGroup[countLine - 1] = ClearText(nodes[1 - offset].InnerText);
                        subject[countLine - 1] = new Text(Lang.LangTypes.ru, ClearText(nodes[2 - offset].InnerText));
                        teacher[countLine - 1] = GetHref(ref document, nodes[3 - offset]);
                        auditorium[countLine - 1] = GetHref(ref document, nodes[4 - offset]);
                        comment[countLine - 1] = GetHref(ref document, nodes[5 - offset]);

                        static Href GetHref(ref HtmlDocument document, HtmlNode startNode) => new() { Text = new Text(Lang.LangTypes.ru, ClearText(startNode.InnerText)), Url = document.DocumentNode.SelectSingleNode(startNode.XPath + "/a[@href]")?.Attributes["href"].Value };

                        if (countLine > 1)
                        {
                            countLine--;
                            offset = 1;
                            document.LoadHtml(lines[++i]);
                            nodes = document.DocumentNode.SelectNodes("/td");
                            goto addInfoLine;
                        }
                        resul.Add((times, numGroup?.Reverse().Where(x => !string.IsNullOrWhiteSpace(x))?.ToArray(), subject?.Reverse().ToArray(), teacher?.Reverse().ToArray(), auditorium?.Reverse().ToArray(), comment.Reverse()?.ToArray()));
                    }
                }
                return resul?.ToArray();
            }
            DateTime[][] dateTimesDays = typePars == TypePars.InstituteFullTime ? GetData(StartEndTime, Days.Count) : Days.Select(x => { if (DateTime.TryParse(new string(x.Item1.Where(y => char.IsDigit(y) || y == '.' || y == ':').ToArray()), out DateTime date)) { return new DateTime[] { date }; } else { return null; } }).ToArray();
            return Days.Select
                ((x, i) =>
                new Day()
                {
                    Name = new Text(Lang.LangTypes.ru, x.Item1),
                    Date = dateTimesDays[i],
                    Lines = x.Item2.Select
                    (x =>
                    new Day.Line()
                    {
                        Auditorium = x.auditorium,
                        Comment = x.comment,
                        NumGroup = x.numGroup,
                        Teacher = x.teacher,
                        TimeStartEnd = x.times,
                        Subject = x.subject,
                    }).ToArray()
                }).ToArray();

            static DateTime[][] GetData(DateTime[] startEndData, int countWeekday)
            {
                if (startEndData != null && startEndData.Length >= 2)
                {
                    DateTime dateTimeStart = startEndData.First();
                    int countDay = (startEndData.Last() - startEndData.First()).Days;
                    return Enumerable.Range(0, countWeekday).Select((x, i) => Enumerable.Range(x, countDay / countWeekday).Select((y, j) => dateTimeStart.AddDays(i + y * countWeekday)).ToArray()).ToArray();
                }
                return Enumerable.Range(0, countWeekday).Select<int, DateTime[]>(x => null).ToArray();
            }
        }

        private static DateTime[] GetInstituteTimesPeriod(HtmlDocument htmlDocument) => htmlDocument.DocumentNode.SelectSingleNode("//div[@id='npe_instance_1103357_npe_content']//b[1]")?.InnerText.Split("по").Select<string, DateTime?>(x => DateTime.TryParse(x, out DateTime resul) ? resul : null).Where(x => x != null)?.Select(x => (DateTime)x).ToArray();
        public enum TypeSchedule
        {
            Text,
            Files
        }
        public struct Href
        {
            public Text Text;
            public string Url;

            public override string ToString() => Text.ToString();
        }
        public class Day
        {
            public Text Name;
            public DateTime[] Date;
            public Line[] Lines;

            public class Line
            {
                public DateTime[] TimeStartEnd;
                public string[] NumGroup;
                public Text[] Subject;
                public Href[] Teacher;
                public Href[] Auditorium;
                public Href[] Comment;

                public override string ToString() => (TimeStartEnd != null ? TimeStartEnd.First().ToString() + "-" + TimeStartEnd.Last().ToString() : "") + (Subject != null ? Subject.First().ToString() : "");
            }

            public override string ToString() => $"{Name} кол-во строк: {Lines?.Length}";
        }
    }
}