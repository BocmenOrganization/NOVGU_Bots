using System.Linq;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;
using HtmlAgilityPack;
using System.Net;
using BotsCore.Moduls.Translate;
using NOVGUBots.Moduls.NOVGU_SiteData.Model.Schedule;
using NOVGUBots.Moduls.NOVGU_SiteData.Interface;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NOVGUBots.Moduls.NOVGU_SiteData.Model
{
    public class Group : IUpdated
    {
        [JsonProperty]
        public string Name { get; private set; }
        [JsonProperty]
        public uint YearReceipt { get; private set; }
        [JsonProperty]
        public Text Direction { get; private set; }
        [JsonProperty]
        public Text Profile { get; private set; }
        [JsonProperty]
        public User[] users { get; private set; }
        [JsonProperty]
        public TableScheduleStudents tableSchedule;

        public Group(TypePars typePars, ParalelSetting paralelSetting, string name, string urlSchedule)
        {
            Name = name;
            var dataParsUser = GetUsers(Name, typePars);
            Direction = dataParsUser.Direction;
            YearReceipt = dataParsUser.YearReceipt;
            Profile = dataParsUser.Profile;
            if ((paralelSetting & ParalelSetting.PeopleGroup) > 0)
                users = dataParsUser.users?.AsParallel()?.Select(x => new User(x.Name, x.Url))?.ToArray();
            else
                users = dataParsUser.users?.Select(x => new User(x.Name, x.Url))?.ToArray();
            tableSchedule = new TableScheduleStudents(urlSchedule, typePars);
        }
        [JsonConstructor]
        private Group() { }
        private static ((string Name, string Url)[] users, Text Direction, uint YearReceipt, Text Profile) GetUsers(string NameGroup, TypePars typePars)
        {
            ((string Name, string Url)[] users, Text Direction, uint YearReceipt, Text Profile) resul = default;
            NameGroup = new string(NameGroup.Where(x => char.IsDigit(x)).ToArray());
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml((new WebClient()).DownloadString($"https://www.novsu.ru/search/groups/i.2500/?page=search&grpname={NameGroup}"));
            HtmlNodeCollection htmlNodesHeadsList = htmlDocument.DocumentNode.SelectNodes("//div[@id='npe_instance_2500_npe_content']//ul");
            HtmlNodeCollection htmlNodesUsersList = htmlDocument.DocumentNode.SelectNodes("//div[@id='npe_instance_2500_npe_content']//table[@class='viewtable']");

            var dataUsers = GetHtmlDataUsers();
            if (dataUsers != default)
            {
                resul.Direction = new Text(Lang.LangTypes.ru, ClearText(dataUsers.direction));
                resul.YearReceipt = dataUsers.YearReceipt;
                resul.Profile = new Text(Lang.LangTypes.ru, ClearText(dataUsers.Profile));
                htmlDocument.LoadHtml(dataUsers.htmlUsers);
                htmlNodesUsersList = htmlDocument.DocumentNode.SelectNodes("//a");
                resul.users = htmlNodesUsersList?.Select(x => (ClearText(x.InnerText), (x.Attributes.Contains("href") ? x.Attributes["href"].Value : null))).ToArray();
            }

            (string direction, string htmlUsers, uint YearReceipt, string Profile) GetHtmlDataUsers()
            {
                if (htmlNodesHeadsList != null && htmlNodesUsersList != null && htmlNodesHeadsList.Count == htmlNodesUsersList.Count)
                {
                    for (int i = 0; i < htmlNodesHeadsList.Count; i++)
                    {
                        if (GetTypePars(htmlNodesHeadsList[i].InnerText) == typePars)
                        {
                            (string Direction, string YearReceipt, string Profile) = GetGroupInfo(htmlNodesHeadsList[i].InnerHtml);
                            return (Direction, htmlNodesUsersList[i].InnerHtml, uint.Parse(YearReceipt), Profile);
                        }
                    }
                }
                return default;

                static (string Direction, string YearReceipt, string Profile) GetGroupInfo(string html)
                {
                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);
                    HtmlNodeCollection htmlNodes = htmlDocument.DocumentNode.SelectNodes("//li");
                    if (htmlNodes.Count >= 4)
                        return (htmlNodes[2].InnerText, new string(htmlNodes[0].InnerText.Where(x => char.IsDigit(x)).ToArray()), htmlNodes[3].InnerText);
                    return default;
                }

                static TypePars GetTypePars(string data)
                {
                    data = data.ToLower();
                    if (data.Contains("колледж"))
                        return TypePars.College;
                    else if (data.Contains("заочная"))
                        return TypePars.InstituteInAbsentia;
                    else
                        return TypePars.InstituteFullTime;
                }
            }
            return resul;
        }
        public override string ToString() => $"{Name} {Direction}";

        public IEnumerable<object> GetData()
        {
            List<object> resul = new() { tableSchedule };
            if (users != null)
                resul.AddRange(users);
            return resul;
        }
        public void SetData(IEnumerable<object> newData) => throw new System.NotImplementedException();
        public List<Text> GetTextsTranslate()
        {
            List<Text> texts = new() { Direction, Profile };
            if (tableSchedule != null)
                texts.AddRange(tableSchedule.GetTextsTranslate());
            return texts;
        }
        public bool Similarity(object e)
        {
            if (e is Group group)
                return group.Name == Name && group.YearReceipt == YearReceipt;
            return false;
        }
        public bool Update(IEnumerable<object> newData, ref List<object> updatedInfo)
        {
            TableScheduleStudents tableSchedule = (TableScheduleStudents)newData?.FirstOrDefault(x => x is TableScheduleStudents);
            object[] users = newData.Where(x => x is User).ToArray();

            var updatedUserInfo = IUpdated.Update(users, users, ref updatedInfo);
            if (updatedUserInfo.stateUpdated)
                users = updatedUserInfo.newData?.Select(x => (User)x).ToArray();
            (bool ifoUpdateTableSchedule, _) = IUpdated.Update(tableSchedule.GetData(), this.tableSchedule.GetData(), ref updatedInfo);
            //TOODO запись
            return updatedUserInfo.stateUpdated || ifoUpdateTableSchedule;
        }
    }
}