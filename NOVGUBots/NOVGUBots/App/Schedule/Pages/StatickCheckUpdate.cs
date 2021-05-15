using BotsCore;
using BotsCore.Bots.Model;
using BotsCore.Moduls.Tables.Services;
using BotsCore.User;
using BotsCore.User.Models;
using NOVGUBots.App.NOVGU_Standart;
using NOVGUBots.Moduls.NOVGU_SiteData;
using NOVGUBots.Moduls.NOVGU_SiteData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NOVGUBots.Moduls.NOVGU_SiteData.Model.Schedule.Hendler;

namespace NOVGUBots.App.Schedule.Pages
{
    public static class StatickCheckUpdate
    {
        private static readonly ModelMarkerTextData Message_TextEditSchedule = new(CretePageSchedule.NameApp, CretePageSchedule.NameTableText, 16);
        static StatickCheckUpdate()
        {
            DataNOVGU.InstituteFullTime.EventUpdateInstitute += SendUpdateInfoStudent;
            DataNOVGU.InstituteInAbsentia.EventUpdateInstitute += SendUpdateInfoStudent;
            DataNOVGU.College.EventUpdateInstitute += SendUpdateInfoStudent;
            DataNOVGU.EventUpdateUserTeachers += SendUpdateInfoTeacher;
        }
        public static void Start() { }

        private static void SendUpdateInfoStudent(List<object> updateInfo, object oldData, object newData)
        {
            var infoUpdate = GetDayUpdateStudent(oldData, newData);
            foreach (var group in infoUpdate)
                SendSchedule(GetSendInfoUser(group.nameGroup, UserRegister.GetNameGroup), group.dayStudents?.Select(x => x.Date.First()).ToArray());
        }

        private static void SendUpdateInfoTeacher(List<object> updateInfo, object oldData, object newData)
        {
            var infoUpdate = GetDayUpdateTeacher(oldData, newData);
            foreach (var user in infoUpdate)
                SendSchedule(GetSendInfoUser(user.userId, UserRegister.GetUser), user.dayStudents?.Select(x => x.Date.First()).ToArray());
        }
        private static ObjectDataMessageInBot[] GetSendInfoUser(string id, Func<ModelUser, string> search)
        {
            List<ObjectDataMessageInBot> resul = new();
            var users = ManagerUser.GetUsers((x) => search.Invoke(x) == id);
            if (users != null)
            {
                foreach (var user in users)
                    if (user.BotsAccount != null)
                        foreach (var botUser in user.BotsAccount)
                            resul.Add(new ObjectDataMessageInBot(user, botUser));
            }
            if (resul.Any())
                return resul.ToArray();
            return null;
        }
        private static void SendSchedule(ObjectDataMessageInBot[] objectDataMessageInBots, DateTime[] dates) => objectDataMessageInBots?.Select(x => StaticData.GetSendMessage(x, dates)).Select(x => { x.Widget = true; x.IsEditOldMessage = false; x.Text = $"{Message_TextEditSchedule.GetText(x.InBot)}\n\n{x.Text}"; return x; }).AsParallel().ForAll(x => ManagerPage.SendDataBot(x, true));
        public static (string nameGroup, DayStudents[] dayStudents)[] GetDayUpdateStudent(object oldData, object newData)
        {
            if (oldData is InstituteCollege[] old && newData is InstituteCollege[] @new)
            {
                var GroupOld = GetAllDays(old);
                var GroupNew = GetAllDays(@new);
                List<(string nameGroup, DayStudents[] dayStudents)> sendUpdateMessegeInfo = new();
                for (int groupItem = 0; groupItem < GroupNew.Count; groupItem++)
                {
                    DayStudents[] foundGroup = GroupOld.FirstOrDefault(x => x.NameGroup == GroupNew[groupItem].NameGroup).day;
                    if (foundGroup != null)
                    {
                        List<DayStudents> updateSendInfoList = new();
                        for (int dayItem = 0; dayItem < GroupNew[groupItem].day.Length; dayItem++)
                        {
                            if (foundGroup.FirstOrDefault(x => SimilarityDay(x, GroupNew[groupItem].day[dayItem])) == null)
                            {
                                updateSendInfoList.Add(GroupNew[groupItem].day[dayItem]);
                            }
                        }
                        if (updateSendInfoList?.Any() ?? false)
                            sendUpdateMessegeInfo.Add((GroupNew[groupItem].NameGroup, updateSendInfoList.ToArray()));
                    }
                }
                if (sendUpdateMessegeInfo.Count > 0)
                    return sendUpdateMessegeInfo.ToArray();
            }
            return null;

            static bool SimilarityDay(DayStudents d1, DayStudents d2)
            {
                if (d1 != null && d2 != null && d1.Similarity(d2) && d1.Lines?.Length == d2.Lines?.Length)
                {
                    foreach (var line in d1.Lines)
                        if (d2.Lines.FirstOrDefault(x => x.Similarity(line)) == null)
                            return false;
                    return true;
                }
                return false;
            }
            static List<(string NameGroup, DayStudents[] day)> GetAllDays(InstituteCollege[] instituteColleges)
            {
                List<(string NameGroup, DayStudents[] day)> resul = new();

                foreach (var Item_instituteCollege in instituteColleges)
                    foreach (var Item_courses in Item_instituteCollege.Courses)
                        foreach (var Item_group in Item_courses.Groups)
                            resul.Add((Item_group.Name, Item_group.tableSchedule.DataTable));

                return resul;
            }
        }
        public static (string userId, DayTeacher[] dayStudents)[] GetDayUpdateTeacher(object oldData, object newData)
        {
            if (oldData is UserTeacher[] old && newData is UserTeacher[] @new)
            {
                List<(string userId, DayTeacher[] dayStudents)> sendUpdateMessegeInfo = new();
                foreach (var teacherDataNew in @new)
                {
                    List<DayTeacher> updateSendInfoList = new();
                    UserTeacher oldDataTeacher = old.FirstOrDefault(x => x.User.IdString == teacherDataNew.User.IdString);
                    if (oldDataTeacher != null)
                    {
                        foreach (var Day in teacherDataNew.Schedule)
                        {
                            if (oldDataTeacher.Schedule.FirstOrDefault(x => SimilarityDay(x, Day)) == null)
                            {
                                updateSendInfoList.Add(Day);
                            }
                        }
                        if (updateSendInfoList?.Any() ?? false)
                            sendUpdateMessegeInfo.Add((teacherDataNew.User.IdString, updateSendInfoList.ToArray()));
                    }
                }
                if (sendUpdateMessegeInfo.Count > 0)
                    return sendUpdateMessegeInfo.ToArray();
            }
            return null;

            static bool SimilarityDay(DayTeacher d1, DayTeacher d2)
            {
                if (d1 != null && d2 != null && d1.Similarity(d2) && d1.Lines?.Length == d2.Lines?.Length)
                {
                    foreach (var line in d1.Lines)
                        if (d2.Lines.FirstOrDefault(x => x.Similarity(line)) == null)
                            return false;
                    return true;
                }
                return false;
            }
        }
    }
}
