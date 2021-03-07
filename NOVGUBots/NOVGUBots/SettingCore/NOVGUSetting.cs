using BotsCore.Moduls.GetSetting.Interface;
using BotsCore.Moduls.Translate;
using Newtonsoft.Json;

namespace NOVGUBots.SettingCore
{
    public static class NOVGUSetting
    {
        public static IObjectSetting objectSetting;
        public static Lang.LangTypes[] langs;
        /// <summary>
        /// LangList - массив Lang.LangTypes
        /// </summary>
        public static void Start(IObjectSetting objectSetting)
        {
            NOVGUSetting.objectSetting = objectSetting;
            langs = JsonConvert.DeserializeObject<Lang.LangTypes[]>(objectSetting.GetValue("LangList"));
        }
    }
}
