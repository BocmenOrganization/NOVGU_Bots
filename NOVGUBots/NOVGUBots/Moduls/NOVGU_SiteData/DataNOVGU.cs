using BotsCore.Moduls;
using BotsCore.Moduls.Translate;
using Newtonsoft.Json;
using NOVGUBots.Moduls.NOVGU_SiteData.Interface;
using NOVGUBots.Moduls.NOVGU_SiteData.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using static NOVGUBots.Moduls.NOVGU_SiteData.Model.SchedulePage;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;

namespace NOVGUBots.Moduls.NOVGU_SiteData
{
    public static class DataNOVGU
    {
        private const string UrlTeachers = "https://www.novsu.ru/univer/timetable/ochn/i.1103357/?page=allTeachersTimetable";
        public const string PatcSaveInfo = "NOVGUParserData";
        public const string NameFileSchedules = "FileSchedules.json";

#pragma warning disable CA2211 // Поля, не являющиеся константами, не должны быть видимыми
        public static ParalelSetting DefaultParalelSetting = ParalelSetting.PeopleTeacher | ParalelSetting.PeopleGroup | ParalelSetting.Course | ParalelSetting.Institute;
#pragma warning restore CA2211 // Поля, не являющиеся константами, не должны быть видимыми

        private static readonly SchedulePage[] schedules = new SchedulePage[]
        {
            new SchedulePage(new(Lang.LangTypes.ru, "Очная форма обучения"), "https://www.novsu.ru/univer/timetable/ochn/", TypePars.InstituteFullTime),
            new SchedulePage(new(Lang.LangTypes.ru, "Заочная форма обучения"), "https://www.novsu.ru/univer/timetable/zaochn/", TypePars.InstituteInAbsentia),
            new SchedulePage(new(Lang.LangTypes.ru, "Колледжи"), "https://www.novsu.ru/univer/timetable/spo/", TypePars.College),
            new SchedulePage(new(Lang.LangTypes.ru, "Сессия"), "https://www.novsu.ru/univer/timetable/session/", TypePars.Session)
        };
        public static UserTeacher[] UserTeachers { get; private set; }
        public static Update EventUpdateUserTeachers { get; set; }

        /// <summary>
        /// Очников (институт)
        /// </summary>
        public static SchedulePage InstituteFullTime
        {
            get
            {
                return schedules[0];
            }
        }
        /// <summary>
        /// Заочников (институт)
        /// </summary>
        public static SchedulePage InstituteInAbsentia
        {
            get
            {
                return schedules[1];
            }
        }
        /// <summary>
        /// Колледж
        /// </summary>
        public static SchedulePage College
        {
            get
            {
                return schedules[2];
            }
        }
        /// <summary>
        /// Сессия
        /// </summary>
        public static SchedulePage Session
        {
            get
            {
                return schedules[3];
            }
        }

        static DataNOVGU()
        {
            var Texts = schedules.Select(x => x.Name);
            if (SettingCore.NOVGUSetting.langs != null)
                foreach (var lang in SettingCore.NOVGUSetting.langs)
                    Text.MultiTranslate(lang, Texts);

            string PatchFile = Path.Combine(PatcSaveInfo, NameFileSchedules);
            if (File.Exists(PatchFile))
                (schedules, UserTeachers) = JsonConvert.DeserializeObject<(SchedulePage[], UserTeacher[])>(File.ReadAllText(PatchFile));
            else
                LoadNewData(ParalelSetting.PeopleTeacher | ParalelSetting.PeopleGroup | ParalelSetting.Course | ParalelSetting.Institute);
        }
        public static void Start() { }
        public static void LoadNewData(ParalelSetting? paralelSetting = null)
        {
            paralelSetting ??= DefaultParalelSetting;

            if (paralelSetting.Value.HasFlag(ParalelSetting.Schedule))
                schedules.AsParallel().ForAll(x => x.UpdateData(GetNewData(x)));
            else
                foreach (var item in schedules)
                    item.UpdateData(GetNewData(item));

            List<object> infoUpdate = new List<object>();
            var NewDataUserTeachers = GetTeachers(new WebClient().DownloadString(UrlTeachers), (ParalelSetting)paralelSetting);
            var UpdateInfoUserTeachers = IUpdated.Update(NewDataUserTeachers, UserTeachers, ref infoUpdate);
            if (UpdateInfoUserTeachers.stateUpdated)
            {
                UserTeachers = UpdateInfoUserTeachers.newData.Select(x => (UserTeacher)x).ToArray();
                if (EventUpdateUserTeachers != null)
                {
                    try
                    {
                        EventUpdateUserTeachers.Invoke(infoUpdate);
                    }
                    catch (Exception e)
                    {
                        EchoLog.Print($"Произошла ошибка при обработки события обновления данных учителей: {e.Message}");
                    }
                }
            }

            if (!Directory.Exists(PatcSaveInfo))
                Directory.CreateDirectory(PatcSaveInfo);

            File.WriteAllText(Path.Combine(PatcSaveInfo, NameFileSchedules), JsonConvert.SerializeObject((schedules, UserTeachers), Formatting.Indented));

            InstituteCollege[] GetNewData(SchedulePage schedule) => ParsInstitute(new WebClient().DownloadString(schedule.Url), schedule.TypeInstitute, (Parser.ParalelSetting)paralelSetting);
        }
        public static SchedulePage GetInfoScheduleInstitute(TypePars type)
        {
            return type switch
            {
                TypePars.InstituteFullTime => InstituteFullTime,
                TypePars.InstituteInAbsentia => InstituteInAbsentia,
                TypePars.College => College,
                TypePars.Session => Session,
                _ => null
            };
        }
    }
}
