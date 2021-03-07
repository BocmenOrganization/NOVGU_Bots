using BotsCore.Moduls.Translate;
using NOVGUBots.Moduls.NOVGU_SiteData.Interface;
using System.Collections.Generic;
using System.Linq;
using static NOVGUBots.Moduls.NOVGU_SiteData.Model.InstituteCollege;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;

namespace NOVGUBots.Moduls.NOVGU_SiteData.Model
{
    public class Course : IUpdated
    {
        public Text Name;
        public Group[] groups;

        public Course(TypePars typePars, ParalelSetting paralelSetting, string name, params (string nameGroup, string UrlSchedule)[] ps)
        {
            Name = new Text(Lang.LangTypes.ru, name);
            if ((paralelSetting & ParalelSetting.Course) > 0)
                groups = ps?.AsParallel()?.Select(x => new Group(typePars, paralelSetting, x.nameGroup, x.UrlSchedule))?.ToArray();
            else
                groups = ps?.Select(x => new Group(typePars, paralelSetting, x.nameGroup, x.UrlSchedule))?.ToArray();
        }

        public IEnumerable<object> GetData() => groups;

        public string GetId() => Name.ToString();

        public IEnumerable<Text> GetTextsTranslate() => new List<Text> { Name };

        public void SetData(IEnumerable<object> newData) => groups = newData.Select(x => (Group)x).ToArray();

        public override string ToString() => $"Название: {Name}, Кол-во групп: {groups.Length}";
    }
}