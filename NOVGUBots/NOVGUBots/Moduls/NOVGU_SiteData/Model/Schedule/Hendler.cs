using BotsCore.Moduls.Translate;
using HtmlAgilityPack;
using Newtonsoft.Json;
using NOVGUBots.Moduls.NOVGU_SiteData.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;

namespace NOVGUBots.Moduls.NOVGU_SiteData.Model.Schedule
{
    public static class Hendler
    {
        private static List<(DateTime[] dates, string, (DateTime[] times, string[] numGroup, Text[] subject, Href[] who, Href[] auditorium, Href[] comment)[])> ParsDayInfo(string url, TypePars typePars)
        {
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml((new WebClient()).DownloadString(url));
            HtmlNodeCollection htmlNodesTable = htmlDocument.DocumentNode.SelectNodes("//table[@class='shedultable']//tr");
            DateTime[] StartEndTime = GetInstituteTimesPeriod(htmlDocument, typePars);
            List<(string, (DateTime[] times, string[] numGroup, Text[] subject, Href[] who, Href[] auditorium, Href[] comment)[])> Days = new();
            if (htmlNodesTable != null)
            {
                for (int i = 1; i < htmlNodesTable.Count; i++)
                {
                    var infoNode = htmlDocument.DocumentNode.SelectSingleNode(htmlNodesTable[i].XPath + $"//td[@rowspan]");
                    if (infoNode != null)
                    {
                        int count = int.Parse(infoNode.Attributes["rowspan"].Value) - 1;
                        Days.Add((ClearText(infoNode.InnerText), GetDayInfo(htmlNodesTable.Skip(i + 1).Take(count).Select(x => x.InnerHtml).ToArray(), typePars)));
                        i += count;
                    }
                }
            }
            DateTime[][] dateTimesDays = (typePars == TypePars.InstituteFullTime || typePars == TypePars.Teacher) ? GetDateDay(StartEndTime, Days?.Select(x => x.Item1).ToArray()) : Days.Select(x => { if (DateTime.TryParse(new string(x.Item1.Where(y => char.IsDigit(y) || y == '.' || y == ':').ToArray()), out DateTime date)) { return new DateTime[] { date }; } else { return null; } }).ToArray();
            return Days?.Select((x, i) => (dateTimesDays[i], x.Item1, x.Item2)).ToList();
        }
        private static (DateTime[] times, string[] numGroup, Text[] subject, Href[] who, Href[] auditorium, Href[] comment)[] GetDayInfo(string[] lines, TypePars typePars)
        {
            byte offsetType = typePars == TypePars.Teacher ? (byte)1 : (byte)0;
            List<(DateTime[] times, string[] numGroup, Text[] subject, Href[] who, Href[] auditorium, Href[] comment)> resul = new();
            for (int i = 0; i < lines.Length; i++)
            {
                int countLine = 1;

                HtmlDocument document = new();
                document.LoadHtml(lines[i]);
                HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("/td");
                var nodeTime = nodes.First();
                if (nodeTime.Attributes.Contains("rowspan"))
                    _ = int.TryParse(nodeTime.Attributes["rowspan"].Value, out countLine);
                (DateTime[] times, string[] numGroup, Text[] subject, Href[] who, Href[] auditorium, Href[] comment) = (null, typePars != TypePars.Teacher ? new string[countLine] : null, new Text[countLine], new Href[countLine], new Href[countLine], new Href[countLine]);
                byte offset = 0;
                times = nodeTime.InnerHtml?.Split("<br>")?.Where(x => !string.IsNullOrWhiteSpace(x))?.Select(x => DateTime.Parse(x))?.ToArray();
            addInfoLine:
                if (typePars != TypePars.Teacher)
                    numGroup[countLine - 1] = ClearText(nodes[1 - offset - offsetType].InnerText);
                subject[countLine - 1] = new Text(Lang.LangTypes.ru, ClearText(nodes[2 - offset - offsetType].InnerText));
                who[countLine - 1] = GetHref(ref document, nodes[3 - offset - offsetType], typePars == TypePars.Teacher, $"{Host}/univer/timetable/ochn/i.1103357/");
                if (!(typePars == TypePars.Teacher) && who[countLine - 1] != null)
                    who[countLine - 1] = new Href() { Text = who[countLine - 1].Text, Url = GetUrlTeacher(who[countLine - 1].Url) };
                auditorium[countLine - 1] = GetHref(ref document, nodes[4 - offset - offsetType], true, $"{Host}/univer/timetable/ochn/i.1103357/");
                comment[countLine - 1] = GetHref(ref document, nodes[5 - offset - offsetType]);

                static Href GetHref(ref HtmlDocument document, HtmlNode startNode, bool LockText = false, string appendUrlHost = null)
                {
                    string text = ClearText(startNode.InnerText);
                    if (!string.IsNullOrWhiteSpace(text))
                        return new() { Text = new Text(Lang.LangTypes.ru, text) { LockTranslator = LockText }, Url = appendUrlHost + document.DocumentNode.SelectSingleNode(startNode.XPath + "/a[@href]")?.Attributes["href"].Value };
                    return null;
                }
                if (countLine > 1)
                {
                    countLine--;
                    offset = 1;
                    document.LoadHtml(lines[++i]);
                    nodes = document.DocumentNode.SelectNodes("/td");
                    goto addInfoLine;
                }
                resul.Add((times, numGroup?.Reverse().Where(x => !string.IsNullOrWhiteSpace(x))?.ToArray(), subject?.Reverse().ToArray(), who?.Reverse().ToArray(), auditorium?.Reverse().ToArray(), comment.Reverse()?.ToArray()));
            }
            return resul?.ToArray();
        }
        private static string GetUrlTeacher(string url)
        {
            if (url == null || !(url.Contains("ora_") && url.Contains('&')))
                return null;
            url = url.Remove(0, url.IndexOf("ora_") + 4);
            url = url.Substring(0, url.IndexOf('&'));
            return $"{Host}/person/{url}";
        }
        private static DateTime[] GetInstituteTimesPeriod(HtmlDocument htmlDocument, TypePars typePars)
        {
            if (typePars != TypePars.Teacher)
                return htmlDocument.DocumentNode.SelectSingleNode("//div[@id='npe_instance_1103357_npe_content']//b[1]")?.InnerText.Split("по").Select<string, DateTime?>(x => DateTime.TryParse(x, out DateTime resul) ? resul : null).Where(x => x != null)?.Select(x => (DateTime)x).ToArray();
            var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@id='npe_instance_1103357_npe_content']/h3");
            if (nodes != null)
            {
                foreach (var item in nodes)
                {
                    if (item.InnerText.Contains("Расписание"))
                    {
                        return ClearText(item.InnerText).Split(' ')?.Select(x => new string(x.Where(y => char.IsDigit(y) || y == '.').ToArray())).Select<string, DateTime?>(x => { if (DateTime.TryParse(x, out DateTime resul)) { return resul; } else { return null; } }).Where(x => x != null)?.Select(x => (DateTime)x).ToArray();
                    }
                }
            }
            return null;
        }
        private static DateTime[][] GetDateDay(DateTime[] startEndData, string[] nameDays)
        {
            if (startEndData != null && startEndData.Length >= 2)
            {
                DayOfWeek[] dayOfWeek = nameDays.Select(x => GetDayOfWeek(x)).ToArray();
                DateTime dateTimeStart = startEndData.First() < startEndData.First() ? startEndData.First() : startEndData.First();
                return Enumerable.Range(0, (startEndData.Last() - startEndData.First()).Days + 1)
                    .Select((x, i) => dateTimeStart.AddDays(i))
                    .Where(x => dayOfWeek.Contains(x.DayOfWeek))?.GroupBy(x => x.DayOfWeek).Select(x => x.ToArray()).ToArray();
            }
            return null;
            static DayOfWeek GetDayOfWeek(string name)
            {
                name = name.ToLower();
                if (name.Contains("пн"))
                    return DayOfWeek.Monday;
                else if (name.Contains("вт"))
                    return DayOfWeek.Tuesday;
                else if (name.Contains("ср"))
                    return DayOfWeek.Wednesday;
                else if (name.Contains("чт"))
                    return DayOfWeek.Thursday;
                else if (name.Contains("пт"))
                    return DayOfWeek.Friday;
                else if (name.Contains("сб") || name.Contains("суббот"))
                    return DayOfWeek.Saturday;
                else
                    return DayOfWeek.Sunday;
            }
        }

        public static DayTeacher[] GetDayTeacher(string url)
        {
            return ParsDayInfo(url, TypePars.Teacher).Select((x, i) =>
            new DayTeacher(new Text(Lang.LangTypes.ru, x.Item2), x.dates,
                x.Item3.Select(x => new LineTeacher(x.times, x.subject, x.auditorium, x.who, x.comment)).ToArray()
            )).ToArray();
        }
        public static DayStudents[] GetDayStudents(string url, TypePars typePars)
        {
            if (typePars == TypePars.Teacher) throw new ArgumentException();
            return ParsDayInfo(url, typePars).Select(x =>
            new DayStudents(new Text(Lang.LangTypes.ru, x.Item2), x.dates,
                x.Item3.Select(x =>
                new LineStudents(x.numGroup, x.times, x.subject, x.auditorium, x.who, x.comment)).ToArray()
            )).ToArray();
        }

        public class Href
        {
            public Text Text;
            public string Url;
            public string GetId() => $"{Text.GetDefaultText()}{Url}";
            public static bool operator ==(Href t1, Href t2) => (t1?.Text == t2?.Text) && (t1?.Url == t2?.Url);
            public static bool operator !=(Href t1, Href t2) => !(t1 == t2);
            public static bool operator ==(Href t1, object t2)
            {
                if (t2 is Href t3)
                    return t1 == t3;
                return false;
            }
            public static bool operator !=(Href t1, object t2) => !(t1 == t2);

            public override string ToString() => Text.GetDefaultText();

            public override bool Equals(object obj)
            {
                if (obj is Href href)
                    return href == this;
                return false;
            }
            public override int GetHashCode() => Url?.GetHashCode() ?? 0;
        }
        public class DayStudents : IUpdated
        {
            [JsonProperty]
            public Text Name { get; private set; }
            [JsonProperty]
            public DateTime[] Date { get; private set; }
            [JsonProperty]
            public LineStudents[] Lines { get; private set; }

            public DayStudents(Text Name, DateTime[] Date, LineStudents[] Lines) => (this.Name, this.Date, this.Lines) = (Name, Date, Lines);
            [JsonConstructor]
            private DayStudents() { }
            public IEnumerable<object> GetData() => Lines;
            public List<Text> GetTextsTranslate()
            {
                List<Text> texts = new();
                if (Lines != null)
                {
                    foreach (var elem in Lines)
                    {
                        texts.AddRange(elem.Auditorium?.Select(x => x?.Text).Where(x => x != null));
                        texts.AddRange(elem.Comment?.Select(x => x?.Text).Where(x => x != null));
                        texts.AddRange(elem.Subject);
                        texts.AddRange(elem.Who?.Select(x => x?.Text).Where(x => x != null));
                    }
                }
                return texts;
            }
            public bool Similarity(object e)
            {
                if (e is DayStudents dayTeacher)
                    return Enumerable.SequenceEqual(dayTeacher.Date, Date) && dayTeacher.Name == Name;
                return false;
            }
            public void SetData(IEnumerable<object> newData) => Lines = newData?.Select(x => (LineStudents)x).ToArray();

            public override string ToString() => $"{Name} кол-во строк: {Lines?.Length}";
        }
        public class DayTeacher : IUpdated
        {
            [JsonProperty]
            public Text Name { get; private set; }
            [JsonProperty]
            public DateTime[] Date { get; private set; }
            [JsonProperty]
            public LineTeacher[] Lines { get; private set; }

            public DayTeacher(Text Name, DateTime[] Date, LineTeacher[] Lines) => (this.Name, this.Date, this.Lines) = (Name, Date, Lines);
            [JsonConstructor]
            private DayTeacher() { }
            public IEnumerable<object> GetData() => Lines;
            public void SetData(IEnumerable<object> newData) => Lines = newData?.Select(x => (LineTeacher)x).ToArray();
            public List<Text> GetTextsTranslate()
            {
                List<Text> texts = new();
                if (Lines != null)
                {
                    foreach (var elem in Lines)
                    {
                        texts.AddRange(elem.Auditorium?.Select(x => x?.Text).Where(x => x != null));
                        texts.AddRange(elem.Comment?.Select(x => x?.Text).Where(x => x != null));
                        texts.AddRange(elem.Subject);
                        texts.AddRange(elem.Who?.Select(x => x?.Text).Where(x => x != null));
                    }
                }
                return texts;
            }

            public override string ToString() => $"{Name} кол-во строк: {Lines?.Length}";

            public bool Similarity(object e)
            {
                if (e is DayTeacher dayTeacher)
                    return Enumerable.SequenceEqual(dayTeacher.Date, Date) && dayTeacher.Name == Name;
                return false;
            }
        }
        public class LineStudents : LineTeacher
        {
            [JsonProperty]
            public string[] NumGroup { get; private set; }
            public LineStudents(string[] NumGroup, DateTime[] TimeStartEnd, Text[] Subject, Href[] Auditorium, Href[] Who, Href[] Comment) : base(TimeStartEnd, Subject, Auditorium, Who, Comment) => this.NumGroup = NumGroup.Distinct().ToArray();
            [JsonConstructor]
            private LineStudents() { }
        }
        public class LineTeacher : IGetId
        {
            [JsonProperty]
            public DateTime[] TimeStartEnd { get; protected set; }
            [JsonProperty]
            public Text[] Subject { get; protected set; }
            [JsonProperty]
            public Href[] Auditorium { get; protected set; }
            [JsonProperty]
            public Href[] Who { get; protected set; }
            [JsonProperty]
            public Href[] Comment { get; protected set; }

            public LineTeacher(DateTime[] TimeStartEnd, Text[] Subject, Href[] Auditorium, Href[] Who, Href[] Comment)
            {
                this.TimeStartEnd = TimeStartEnd;
                this.Subject = Subject.Distinct().ToArray();
                this.Auditorium = Auditorium.Distinct().ToArray();
                this.Who = Who.Distinct().ToArray();
                this.Comment = Comment.Distinct().ToArray();
            }
            [JsonConstructor]
            protected LineTeacher() { }
            public bool Similarity(object e)
            {
                if (e is LineTeacher NewlineTeacher)
                    return Enumerable.SequenceEqual(NewlineTeacher.TimeStartEnd, TimeStartEnd) && Enumerable.SequenceEqual(NewlineTeacher.Subject, Subject) && Enumerable.SequenceEqual(NewlineTeacher.Auditorium, Auditorium) && Enumerable.SequenceEqual(NewlineTeacher.Who, Who) && Enumerable.SequenceEqual(NewlineTeacher.Comment, Comment);
                return false;
            }



            public override string ToString() => (TimeStartEnd != null ? TimeStartEnd.First().ToString() + "-" + TimeStartEnd.Last().ToString() : "") + (Subject != null ? Subject.First().ToString() : "");
        }
    }
}