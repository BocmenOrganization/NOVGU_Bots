using System.Linq;
using static NOVGUBots.Moduls.NOVGU_SiteData.Model.InstituteCollege;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;
using HtmlAgilityPack;
using System.Net;
using BotsCore.Moduls.Translate;

namespace NOVGUBots.Moduls.NOVGU_SiteData.Model
{
    public class Group
    {
        public string Name;
        public uint YearReceipt;
        public Text Direction;
        public Text Profile;
        public User[] users;
        public TableSchedule tableSchedule;
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
            tableSchedule = new TableSchedule(urlSchedule, typePars);
        }
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
    }
}