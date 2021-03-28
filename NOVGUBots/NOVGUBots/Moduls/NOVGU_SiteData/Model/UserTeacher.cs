using BotsCore.Moduls.Translate;
using Newtonsoft.Json;
using NOVGUBots.Moduls.NOVGU_SiteData.Interface;
using System.Collections.Generic;
using System.Linq;
using static NOVGUBots.Moduls.NOVGU_SiteData.Model.Schedule.Hendler;

namespace NOVGUBots.Moduls.NOVGU_SiteData.Model
{
    public class UserTeacher : IUpdated
    {
        [JsonProperty]
        public User User { get; private set; }
        [JsonProperty]
        public DayTeacher[] Schedule { get; private set; }
        public UserTeacher(string url, string name)
        {
            url = $"/univer/timetable/ochn/i.1103357/{url}";
            User = new User(name, new string(url.Remove(0, url.LastIndexOf('=')).Where(x => char.IsDigit(x)).ToArray()));
            Schedule = GetDayTeacher($"{Parser.Host}{url}");
        }
        [JsonConstructor]
        private UserTeacher() { }
        public override string ToString() => User.Name;

        public bool Similarity(object e)
        {
            if (e is UserTeacher userTeacher)
                return userTeacher.User.Similarity(User);
            return false;
        }
        public IEnumerable<object> GetData() => Schedule;
        public void SetData(IEnumerable<object> newData) => Schedule = newData.Select(x => (DayTeacher)x).ToArray();
        public List<Text> GetTextsTranslate()
        {
            List<Text> texts = new List<Text>();
            if (Schedule!= null)
                foreach (var elem in Schedule)
                    texts.AddRange(elem.GetTextsTranslate());
            return texts;
        }
    }
}
