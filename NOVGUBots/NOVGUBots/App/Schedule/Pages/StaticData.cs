using BotsCore.Moduls.Tables.Services;
using NOVGUBots.Moduls.NOVGU_SiteData;
using System;
using static BotsCore.Bots.Model.ObjectDataMessageSend;
using System.Linq;
using BotsCore.Bots.Model;
using NOVGUBots.App.NOVGU_Standart;
using System.Text;
using static NOVGUBots.Moduls.NOVGU_SiteData.Model.Schedule.Hendler;
using System.Collections.Generic;
using BotsCore.Moduls.Translate;

namespace NOVGUBots.App.Schedule.Pages
{
    public static class StaticData
    {
        public static readonly ModelMarkerTextData Message_TextUpWeek = new(CretePageSchedule.NameApp, CretePageSchedule.NameTableText, 10);
        public static readonly ModelMarkerTextData Message_TextDownWeek = Message_TextUpWeek.GetElemNewId(11);
        public static readonly ModelMarkerUneversalData<Media[]> MessageMedia_UpWeek = new(CretePageSchedule.NameApp, CretePageSchedule.NameTableMedia, 0);
        public static readonly ModelMarkerUneversalData<Media[]> MessageMedia_DownWeek = MessageMedia_UpWeek.GetElemNewId(1);
        public static readonly ModelMarkerTextData Text_NoDataSchedule = Message_TextUpWeek.GetElemNewId(12);
        //====================================================== Фото дней недели
        public static ModelMarkerUneversalData<Media[]>[][] MessageMedia_DayOfWeek = new ModelMarkerUneversalData<Media[]>[][]
        {
            new ModelMarkerUneversalData<Media[]>[]
            {
                MessageMedia_UpWeek.GetElemNewId(2),
                MessageMedia_UpWeek.GetElemNewId(3)
            },
            new ModelMarkerUneversalData<Media[]>[]
            {
                MessageMedia_UpWeek.GetElemNewId(4),
                MessageMedia_UpWeek.GetElemNewId(5)
            },
            new ModelMarkerUneversalData<Media[]>[]
            {
                MessageMedia_UpWeek.GetElemNewId(6),
                MessageMedia_UpWeek.GetElemNewId(7)
            },
            new ModelMarkerUneversalData<Media[]>[]
            {
                MessageMedia_UpWeek.GetElemNewId(8),
                MessageMedia_UpWeek.GetElemNewId(9)
            },
            new ModelMarkerUneversalData<Media[]>[]
            {
                MessageMedia_UpWeek.GetElemNewId(10),
                MessageMedia_UpWeek.GetElemNewId(11)
            },
            new ModelMarkerUneversalData<Media[]>[]
            {
                MessageMedia_UpWeek.GetElemNewId(12),
                MessageMedia_UpWeek.GetElemNewId(13)
            },
        };
        //====================================================== Дни недели
        public static readonly ModelMarkerTextData[] DayOfWeek = new ModelMarkerTextData[]
        {
            Message_TextUpWeek.GetElemNewId(1),
            Message_TextUpWeek.GetElemNewId(2),
            Message_TextUpWeek.GetElemNewId(3),
            Message_TextUpWeek.GetElemNewId(4),
            Message_TextUpWeek.GetElemNewId(5),
            Message_TextUpWeek.GetElemNewId(6)
        };
        //====================================================== Эмодзи
        private static readonly ModelMarkerStringlData Emoji_time = new(CretePageSchedule.NameApp, CretePageSchedule.NameTableString, 10);
        private static readonly ModelMarkerStringlData Emoji_teacher = Emoji_time.GetElemNewId(11);
        private static readonly ModelMarkerStringlData Emoji_auditorium = Emoji_time.GetElemNewId(12);
        private static readonly ModelMarkerStringlData Emoji_comments = Emoji_time.GetElemNewId(13);
        private static readonly ModelMarkerStringlData Emoji_students = Emoji_time.GetElemNewId(14);

        private static readonly ModelMarkerStringlData Emoji_newLineAppend = Emoji_teacher.GetElemNewId(15);
        private static readonly ModelMarkerStringlData Emoji_newLineDown = Emoji_teacher.GetElemNewId(16);
        private static readonly ModelMarkerStringlData Emoji_newLineUp = Emoji_teacher.GetElemNewId(17);
        private static readonly ModelMarkerStringlData Emoji_newLineNull = Emoji_teacher.GetElemNewId(18);
        //====================================================== Набор эмодзи для составления номера
        private static readonly ModelMarkerStringlData[] Nembers_One = new ModelMarkerStringlData[]
        {
            Emoji_time.GetElemNewId(0),
            Emoji_time.GetElemNewId(1),
            Emoji_time.GetElemNewId(2),
            Emoji_time.GetElemNewId(3),
            Emoji_time.GetElemNewId(4),
            Emoji_time.GetElemNewId(5),
            Emoji_time.GetElemNewId(6),
            Emoji_time.GetElemNewId(7),
            Emoji_time.GetElemNewId(8),
            Emoji_time.GetElemNewId(9)
        };

        /// <summary>
        /// Определяет какая неделя верхняя или нижняя
        /// </summary>
        /// <returns>true - верхняя, false - нижняя</returns>
        public static bool GetInfo_UpDownWeek()
        {
            DateTime thisDate = GetThisMonday();
            if (DataNOVGU.Calendar?.First()?.FirstOrDefault(x => thisDate >= x.First() && x.Last() >= thisDate) != default)
                return true;
            return false;
        }
        /// <summary>
        /// Получения даты понедельника на этой неделе
        /// </summary>
        /// <param name="StartDate">Опорный день поиска, null - автоматически будет выбран текущий день</param>
        public static DateTime GetThisMonday(DateTime? StartDate = null)
        {
            StartDate ??= DateTime.Now;
            return StartDate.Value.AddDays(StartDate.Value.DayOfWeek == 0 ? -6 : ((-(int)StartDate.Value.DayOfWeek) + 1));
        }
        public static DateTime[] FilterDays(ObjectDataMessageInBot inBot, params DateTime[] dates)
        {
            if (dates == null) return null;
            if (UserRegister.GetUserState(inBot) == UserRegister.UserState.Student)
            {
                string NameInstituteCollege = UserRegister.GetNameInstituteCollege(inBot);
                string NameCourse = UserRegister.GetNameCourse(inBot);
                string NameGroup = UserRegister.GetNameGroup(inBot);
                var ScheduleO = DataNOVGU.GetInfoScheduleInstitute
                (
                UserRegister.GetTypeSchedule(inBot)
                )
                    .Institute
                    ?.FirstOrDefault(x => x.Name.GetDefaultText() == NameInstituteCollege)
                    ?.Courses
                    ?.FirstOrDefault(x => x.Name.GetDefaultText() == NameCourse)
                    ?.Groups
                    ?.FirstOrDefault(x => x.Name == NameGroup)
                    .tableSchedule
                    ?.DataTable;
                return dates.Where(x => ScheduleO.Where(o => o.Date.Where(d => d.Date == x).Any()).Any()).ToArray();
            }
            else
            {
                string NameIdUser = UserRegister.GetUser(inBot);
                var ScheduleO = DataNOVGU.UserTeachers?.FirstOrDefault(x => x.User.IdString == NameIdUser)
                    ?.Schedule;
                return dates.Where(x => ScheduleO.Where(o => o.Date.Where(d => d.Date == x).Any()).Any()).ToArray();
            }
        }

        public static ObjectDataMessageSend GetSendMessage(ObjectDataMessageInBot inBot, params DateTime[] dates)
        {
            Func<Href, Lang.LangTypes, string> generateUrlText = inBot.BotID.bot == BotsCore.Bots.Interface.IBot.BotTypes.Telegram ? GenereteUrlTelegram : null;
            return GetSendMessage(inBot, generateUrlText, dates);
        }
        public static ObjectDataMessageSend GetSendMessage(ObjectDataMessageInBot inBot, Func<Href, Lang.LangTypes, string> generateUrlText, params DateTime[] dates)
        {
            Media[] medias = null;
            if (dates?.Length == 1)
                medias = MessageMedia_DayOfWeek[(int)dates.First().DayOfWeek - 1][GetInfo_UpDownWeek() ? 0 : 1];
            return new ObjectDataMessageSend(inBot) { Text = GetScheduleText(inBot, generateUrlText, dates), media = medias };
        }
        /// <summary>
        /// Генерация текста с расписанием
        /// </summary>
        /// <param name="GenerateUrlText">Форматирование ссылок, null - если вставка ссылки не требуется</param>
        /// <param name="dates">Даты для которых нужно собрать расписание</param>
        public static string GetScheduleText(ObjectDataMessageInBot inBot, Func<Href, Lang.LangTypes, string> GenerateUrlText, params DateTime[] dates)
        {
            if (dates == null || dates.Length == 0) return null;
            StringBuilder stringBuilder = new();
            (DateTime[], string)[] daysSchedule = null;
            if (UserRegister.GetUserState(inBot) == UserRegister.UserState.Student)
                daysSchedule = GetScheduleTextStudent(inBot, GenerateUrlText, dates);
            else
                daysSchedule = GetScheduleTextTeacher(inBot, GenerateUrlText, dates);
            if (daysSchedule != null && daysSchedule.Length > 0)
            {
                if (dates.Length == 1) return daysSchedule.First().Item2;
                string[] lines = new string[daysSchedule.Length];
                for (int i = 0; i < daysSchedule.Length; i++)
                {
                    lines[i] = $"{Emoji_newLineUp}{DayOfWeek[(int)daysSchedule[i].Item1.First().DayOfWeek - 1].GetText(inBot)} -> {string.Join(" ", daysSchedule[i].Item1.Select(x => x.ToShortDateString()))}\n{BuildLines(daysSchedule[i].Item2.Split('\n'))}";
                }
                return string.Join("\n\n", lines);
            }
            else
                return Text_NoDataSchedule.GetText(inBot);
        }
        /// <summary>
        /// Получение расписания дней студента
        /// </summary>
        /// <param name="generateUrlText">Форматирование ссылок, null - если вставка ссылки не требуется</param>
        public static (DateTime[], string)[] GetScheduleTextStudent(ObjectDataMessageInBot inBot, Func<Href, Lang.LangTypes, string> generateUrlText, params DateTime[] dates)
        {
            if (dates == null || dates.Length == 0) return null;
            string NameInstituteCollege = UserRegister.GetNameInstituteCollege(inBot);
            string NameCourse = UserRegister.GetNameCourse(inBot);
            string NameGroup = UserRegister.GetNameGroup(inBot);

            var ScheduleO = DataNOVGU.GetInfoScheduleInstitute
                (
                UserRegister.GetTypeSchedule(inBot)
                )
                .Institute
                ?.FirstOrDefault(x => x.Name.GetDefaultText() == NameInstituteCollege)
                ?.Courses
                ?.FirstOrDefault(x => x.Name.GetDefaultText() == NameCourse)
                ?.Groups
                ?.FirstOrDefault(x => x.Name == NameGroup)
                .tableSchedule
                ?.DataTable
                ?.Where(x => (x.Date?.Where(x => dates.Where(d => d.Date == x.Date).Any())?.Count() ?? 0) > 0)?.ToArray();

            if (ScheduleO != null)
                return ScheduleO?.Select(s => (s.Date, string.Join("\n\n", s.Lines?.Select((x, i) => $"{GetNumber((uint)(i + 1))}{x.Subject?.First().GetText(inBot)}\n{BuildLines(GetLinesSchedule(inBot, x, generateUrlText, Emoji_teacher))}")))).ToArray();
            return null;
        }
        /// <summary>
        /// Получение расписания для преподователей
        /// </summary>
        /// <param name="generateUrlText">Форматирование ссылок, null - если вставка ссылки не требуется</param>
        public static (DateTime[], string)[] GetScheduleTextTeacher(ObjectDataMessageInBot inBot, Func<Href, Lang.LangTypes, string> generateUrlText, params DateTime[] dates)
        {
            if (dates == null || dates.Length == 0) return null;
            string NameIdUser = UserRegister.GetUser(inBot);
            var ScheduleO = DataNOVGU.UserTeachers?.FirstOrDefault(x => x.User.IdString == NameIdUser)
                ?.Schedule
                ?.Where(x => x.Date?.Where(x => dates.Where(d => d.Date == x.Date).Any()).Count() > 0)?.ToArray();
            if (ScheduleO != null)
                return ScheduleO?.Select(s => (s.Date, string.Join("\n\n", s.Lines?.Select((x, i) => $"{GetNumber((uint)(i + 1))}{x.Subject?.First().GetText(inBot)}\n{BuildLines(GetLinesSchedule(inBot, x, generateUrlText, Emoji_students))}")))).ToArray();
            return null;
        }
        /// <summary>
        /// Генерация строк для строки расписания
        /// </summary>
        private static string[] GetLinesSchedule(ObjectDataMessageInBot inBot, LineTeacher lineTeacher, Func<Href, Lang.LangTypes, string> generateUrlText, string EmojiWho)
        {
            if (lineTeacher == null) return null;
            List<string> linesRes = new();
            if (lineTeacher.TimeStartEnd != null && lineTeacher.TimeStartEnd.Any())
                linesRes.Add($"{Emoji_time}{lineTeacher.TimeStartEnd.First():hh\\:mm}-{new TimeSpan(lineTeacher.TimeStartEnd.Last().Hours, 45, 0):hh\\:mm}");
            if (lineTeacher.Who != null && lineTeacher.Who.Any())
                linesRes.AddRange(lineTeacher.Who.Where(x => x != null).Select(x => $"{EmojiWho} {generateUrlText?.Invoke(x, inBot) ?? x.Text.GetText(inBot)}"));
            if (lineTeacher.Auditorium != null && lineTeacher.Auditorium.Any())
                linesRes.AddRange(lineTeacher.Auditorium?.Where(x => x != null).Select(x => $"{Emoji_auditorium} {generateUrlText?.Invoke(x, inBot) ?? x.Text.GetText(inBot)}"));
            if (lineTeacher.Comment != null && lineTeacher.Comment.Any())
                linesRes.AddRange(lineTeacher.Comment.Where(x => x != null).Select(x => $"{Emoji_comments} {generateUrlText?.Invoke(x, inBot) ?? x.Text.GetText(inBot)}"));
            return linesRes?.Where(x => x != null && lineTeacher.TimeStartEnd.Any())?.ToArray();
        }
        /// <summary>
        /// Генерация вложенного текста
        /// </summary>
        private static string BuildLines(string[] lines) => string.Join("\n", lines.Select((x, i) => $"{((lines.Length == (i + 1)) ? Emoji_newLineDown : (string.IsNullOrWhiteSpace(x) ? Emoji_newLineNull : Emoji_newLineAppend))}{x}"));
        /// <summary>
        /// Генерация числа из эмодзи
        /// </summary>
        private static string GetNumber(uint num)
        {
            StringBuilder stringBuilder = new();
            string number = num.ToString();
            foreach (var @cahr in number)
                stringBuilder.Append(GetNum(@cahr));
            static string GetNum(char c)
            {
                return c switch
                {
                    '0' => Nembers_One[0],
                    '1' => Nembers_One[1],
                    '2' => Nembers_One[2],
                    '3' => Nembers_One[3],
                    '4' => Nembers_One[4],
                    '5' => Nembers_One[5],
                    '6' => Nembers_One[6],
                    '7' => Nembers_One[7],
                    '8' => Nembers_One[8],
                    '9' => Nembers_One[9],
                    _ => throw new NotImplementedException()
                };
            }
            return stringBuilder.ToString();
        }
        private static string GenereteUrlTelegram(Href href, Lang.LangTypes lang)
        {
            if (href != null && href.Text != null && !string.IsNullOrWhiteSpace(href.Url))
                return $"[{href.Text.GetText(lang)}]({href.Url})";
            return null;
        }
    }
}