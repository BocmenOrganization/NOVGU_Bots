using BotsCore.Moduls.Translate;
using Newtonsoft.Json;
using NOVGUBots.Moduls.NOVGU_SiteData.Interface;
using System.Collections.Generic;
using System.Linq;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;

namespace NOVGUBots.Moduls.NOVGU_SiteData.Model
{
    public class Course : IUpdated
    {
        [JsonProperty]
        public Text Name { get; private set; }
        [JsonProperty]
        public Group[] groups { get; private set; }

        public Course(TypePars typePars, ParalelSetting paralelSetting, string name, params (string nameGroup, string UrlSchedule)[] ps)
        {
            Name = new Text(Lang.LangTypes.ru, name);
            if ((paralelSetting & ParalelSetting.Course) > 0)
                groups = ps?.AsParallel()?.Select(x => new Group(typePars, paralelSetting, x.nameGroup, x.UrlSchedule))?.ToArray();
            else
                groups = ps?.Select(x => new Group(typePars, paralelSetting, x.nameGroup, x.UrlSchedule))?.ToArray();
        }
        [JsonConstructor]
        private Course() { }
        public override string ToString() => $"Название: {Name}, Кол-во групп: {groups.Length}";

        public IEnumerable<object> GetData() => groups;
        public List<Text> GetTextsTranslate()
        {
            List<Text> texts = new List<Text> { Name };
            if (groups != null)
                foreach (var item in groups)
                    texts.AddRange(item.GetTextsTranslate());
            return texts;
        }
        public void SetData(IEnumerable<object> newData) => groups = newData?.Select(x => (Group)x).ToArray();
        public bool Similarity(object e)
        {
            if (e is Course course)
                return course.Name == Name;
            return false;
        }
    }
}