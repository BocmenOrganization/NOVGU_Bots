using BotsCore.Moduls;
using BotsCore.Moduls.Translate;
using BotsCore.User.Models;
using Newtonsoft.Json;
using NOVGUBots.App.NOVGU_Standart;
using NOVGUBots.Moduls.NOVGU_SiteData.Interface;
using NOVGUBots.Moduls.NOVGU_SiteData.Model;
using NOVGUBots.SettingCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static NOVGUBots.Moduls.NOVGU_SiteData.Model.SchedulePage;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;

namespace NOVGUBots.Moduls.NOVGU_SiteData
{
    public static class DataNOVGU
    {
        private const string UrlCalendar = "https://www.novsu.ru/study/";
        private const string UrlTeachers = "https://www.novsu.ru/univer/timetable/ochn/i.1103357/?page=allTeachersTimetable";
        public const string PatcSaveInfo = "NOVGUParserData";
        public const string NameFileSchedules = "FileSchedules.json";

#pragma warning disable CA2211 // Поля, не являющиеся константами, не должны быть видимыми
        public static ParalelSetting DefaultParalelSetting = ParalelSetting.PeopleTeacher | ParalelSetting.PeopleGroup | ParalelSetting.Course | ParalelSetting.Group;
#pragma warning restore CA2211 // Поля, не являющиеся константами, не должны быть видимыми

        private static readonly SchedulePage[] schedules = new SchedulePage[]
        {
            new SchedulePage(new(Lang.LangTypes.ru, "Очная форма обучения"), "https://www.novsu.ru/univer/timetable/ochn/", TypePars.InstituteFullTime),
            new SchedulePage(new(Lang.LangTypes.ru, "Заочная форма обучения"), "https://www.novsu.ru/univer/timetable/zaochn/", TypePars.InstituteInAbsentia),
            new SchedulePage(new(Lang.LangTypes.ru, "Колледжи"), "https://www.novsu.ru/univer/timetable/spo/", TypePars.College),
            new SchedulePage(new(Lang.LangTypes.ru, "Сессия"), "https://www.novsu.ru/univer/timetable/session/", TypePars.Session)
        };
        private static System.Timers.Timer timerUpdate;
        public static UserTeacher[] UserTeachers { get; private set; } = Array.Empty<UserTeacher>();
        public static Update EventUpdateUserTeachers { get; set; }
        public static DateTime[][][] Calendar { get; private set; }

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
            if (NOVGUSetting.langs != null)
                foreach (var lang in NOVGUSetting.langs)
                    Text.MultiTranslate(lang, Texts);

            string PatchFile = Path.Combine(PatcSaveInfo, NameFileSchedules);
            if (File.Exists(PatchFile))
                (schedules, UserTeachers, Calendar) = JsonConvert.DeserializeObject<(SchedulePage[], UserTeacher[], DateTime[][][])>(File.ReadAllText(PatchFile));
            else
                LoadNewData(DefaultParalelSetting, NOVGUSetting.langs);
        }
        public static void Start(uint periodUpdate = 1000)
        {
            //timerUpdate?.Stop();
            //timerUpdate = new System.Timers.Timer(periodUpdate);
            //timerUpdate.Elapsed += (sender, e) =>
            //{
            //    try
            //    {
            //        LoadNewData(DefaultParalelSetting, NOVGUSetting.langs);
            //    }
            //    catch (Exception ex)
            //    {
            //        EchoLog.Print($"При обновлении расписания произошла ошибка: {ex.Message}");
            //    }
            //    timerUpdate?.Start();
            //};
            //timerUpdate.AutoReset = false;
            //timerUpdate.Start();
        }
        public static void LoadNewData(ParalelSetting? paralelSetting, params Lang.LangTypes[] langs)
        {
            paralelSetting ??= DefaultParalelSetting;

            if (paralelSetting.Value.HasFlag(ParalelSetting.Schedule))
                schedules.AsParallel().ForAll(x => x.UpdateData(GetNewData(x), langs));
            else
                foreach (var item in schedules)
                    item.UpdateData(GetNewData(item), langs);

            List<object> infoUpdate = new();
            var NewDataUserTeachers = GetTeachers(LoadHtml(UrlTeachers), (ParalelSetting)paralelSetting);
            var OldDataTescher = JsonConvert.DeserializeObject<UserTeacher[]>(JsonConvert.SerializeObject(UserTeachers));
            var (stateUpdated, newData) = IUpdated.Update(NewDataUserTeachers, UserTeachers, ref infoUpdate);
            if (stateUpdated)
            {
                UserTeacher[] newdataTeacher = newData.Select(x => (UserTeacher)x).ToArray();
                newdataTeacher.AsParallel().ForAll(x => ((ITranslatable)x).Translate(langs));
                if (EventUpdateUserTeachers != null)
                {
                    try
                    {
                        EventUpdateUserTeachers.Invoke(infoUpdate, OldDataTescher, newdataTeacher);
                        EchoLog.Print("Обновилось расписание преподователей");
                    }
                    catch (Exception e)
                    {
                        EchoLog.Print($"Произошла ошибка при обработки события обновления данных учителей: {e.Message}");
                    }
                }
                lock (UserTeachers)
                {
                    UserTeachers = NewDataUserTeachers;
                }
            }

            Calendar = GetCalendar(LoadHtml(UrlCalendar));

            if (!Directory.Exists(PatcSaveInfo))
                Directory.CreateDirectory(PatcSaveInfo);

            File.WriteAllText(Path.Combine(PatcSaveInfo, NameFileSchedules), JsonConvert.SerializeObject((schedules, UserTeachers, Calendar), Formatting.Indented));

            InstituteCollege[] GetNewData(SchedulePage schedule) => ParsInstitute(LoadHtml(schedule.Url), schedule.TypeInstitute, (Parser.ParalelSetting)paralelSetting);
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

        public static object GetScheduleUser(ModelUser user)
        {
            if (UserRegister.GetInfoRegisterUser(user).HasFlag(UserRegister.RegisterState.GroupOrTeacherSet))
            {
                if (UserRegister.GetUserState(user) == UserRegister.UserState.Student)
                {
                    string NameInstituteCollege = UserRegister.GetNameInstituteCollege(user);
                    string NameCourse = UserRegister.GetNameCourse(user);
                    string NameGroup = UserRegister.GetNameGroup(user);
                    return GetInfoScheduleInstitute(UserRegister.GetTypeSchedule(user)).Institute?.FirstOrDefault(x => x.Name.GetDefaultText() == NameInstituteCollege)?.Courses?.FirstOrDefault(x => x.Name.GetDefaultText() == NameCourse)?.Groups?.FirstOrDefault(x => x.Name == NameGroup)?.tableSchedule;
                }
                else
                {
                    string NameIdUser = UserRegister.GetUser(user);
                    return UserTeachers?.FirstOrDefault(x => x.User.IdString == NameIdUser)?.Schedule;
                }
            }
            return null;
        }
    }
}
