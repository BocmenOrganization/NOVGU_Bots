using BotsCore.Moduls;
using BotsCore.Moduls.Translate;
using NOVGUBots.Moduls.NOVGU_SiteData.Interface;
using NOVGUBots.Moduls.NOVGU_SiteData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static NOVGUBots.Moduls.NOVGU_SiteData.Model.InstituteCollege;

namespace NOVGUBots.Moduls.NOVGU_SiteData
{
    public static class DataNOVGU
    {
#pragma warning disable CA2211 // Поля, не являющиеся константами, не должны быть видимыми
        public static Parser.ParalelSetting DefaultParalelSetting = Parser.ParalelSetting.PeopleTeacher | Parser.ParalelSetting.PeopleGroup | Parser.ParalelSetting.Course | Parser.ParalelSetting.Institute;
        public delegate void Update(TypePars typePars, List<object> updateInfo);

        public static Text NameInstituteFullTime = new(Lang.LangTypes.ru, "Очная форма обучения");
        public const string UrlInstituteFullTime = "https://www.novsu.ru/univer/timetable/ochn/";
        public static Update EventUpdateInstituteFullTime;
        public static InstituteCollege[] InstituteFullTime = Array.Empty<InstituteCollege>();

        public static Text NameInstituteInAbsentia = new(Lang.LangTypes.ru, "Заочная форма обучения");
        public const string UrlInstituteInAbsentia = "https://www.novsu.ru/univer/timetable/zaochn/";
        public static Update EventUpdateInstituteInAbsentia;
        public static InstituteCollege[] InstituteInAbsentia = Array.Empty<InstituteCollege>();

        public static Text NameCollege = new(Lang.LangTypes.ru, "Колледжи");
        public const string UrlCollege = "https://www.novsu.ru/univer/timetable/spo/";
        public static Update EventUpdateCollege;
        public static InstituteCollege[] College = Array.Empty<InstituteCollege>();

        public static Text NameSession = new(Lang.LangTypes.ru, "Сессия");
        public const string UrlSession = "https://www.novsu.ru/univer/timetable/session/";
        public static Update EventUpdateSession;
        public static InstituteCollege[] Session = Array.Empty<InstituteCollege>();
#pragma warning restore CA2211 // Поля, не являющиеся константами, не должны быть видимыми

        public static void LoadNewData(Parser.ParalelSetting? paralelSetting = null)
        {
            paralelSetting ??= DefaultParalelSetting;
            LoadInstituteData(TypePars.InstituteFullTime, (Parser.ParalelSetting)paralelSetting);
        }

        private static void LoadInstituteData(TypePars type, Parser.ParalelSetting paralelSetting)
        {
            List<object> listUpdate = null;
            InstituteCollege[] newData = Parser.ParsInstitute((new WebClient()).DownloadString(GetUrlInstitute(type)), type, paralelSetting);
            IEnumerable<object> df = GetInstitute(type);
            if (!df.Any())
            {
                bool isUpdate = false;
                (isUpdate, df) = IUpdated.Update(newData, df, ref listUpdate);
                if (isUpdate)
                    SetDataInstitute(type, df.Select(x => (InstituteCollege)x).ToArray(), listUpdate);
            }
            else
                SetDataInstitute(type, newData);
        }
        private static IEnumerable<InstituteCollege> GetInstitute(TypePars type)
        {
#pragma warning disable CS8524 // Выражение switch не обрабатывает некоторые типы входных значений, в том числе неименованное значение перечисления (не является исчерпывающим).
            return type switch
#pragma warning restore CS8524 // Выражение switch не обрабатывает некоторые типы входных значений, в том числе неименованное значение перечисления (не является исчерпывающим).
            {
                TypePars.InstituteFullTime => InstituteFullTime,
                TypePars.InstituteInAbsentia => InstituteInAbsentia,
                TypePars.College => College,
                TypePars.Session => Session
            };
        }
        private static void SetDataInstitute(TypePars type, InstituteCollege[] newData, List<object> updateInfo = null)
        {
            switch (type)
            {
                case TypePars.InstituteFullTime:
                    Update(ref InstituteFullTime, EventUpdateInstituteFullTime, NameInstituteFullTime);
                    break;
                case TypePars.InstituteInAbsentia:
                    Update(ref InstituteInAbsentia, EventUpdateInstituteInAbsentia, NameInstituteInAbsentia);
                    break;
                case TypePars.College:
                    Update(ref College, EventUpdateCollege, NameCollege);
                    break;
                case TypePars.Session:
                    Update(ref Session, EventUpdateSession, NameSession);
                    break;
            }
            void Update(ref InstituteCollege[] instituteColleges, Update update, Text nameInstituteColleg)
            {
#pragma warning disable CS0728 // Возможно, используется недопустимое назначение для локального параметра, который является аргументом оператора using или lock
                lock (instituteColleges) { instituteColleges = newData; }
#pragma warning restore CS0728 // Возможно, используется недопустимое назначение для локального параметра, который является аргументом оператора using или lock
                if (EventUpdateInstituteFullTime != null)
                {
                    try
                    {
                        EventUpdateInstituteFullTime.Invoke(type, updateInfo);
                    }
                    catch (Exception e)
                    {
                        EchoLog.Print($"Произошла ошибка при обработки события обновления {nameInstituteColleg}: {e.Message}");
                    }
                }
            }
        }
        private static string GetUrlInstitute(TypePars type)
        {
#pragma warning disable CS8524 // Выражение switch не обрабатывает некоторые типы входных значений, в том числе неименованное значение перечисления (не является исчерпывающим).
            return type switch
#pragma warning restore CS8524 // Выражение switch не обрабатывает некоторые типы входных значений, в том числе неименованное значение перечисления (не является исчерпывающим).
            {
                TypePars.InstituteFullTime => UrlInstituteFullTime,
                TypePars.InstituteInAbsentia => UrlInstituteInAbsentia,
                TypePars.College => UrlCollege,
                TypePars.Session => UrlSession
            };
        }
    }
}
