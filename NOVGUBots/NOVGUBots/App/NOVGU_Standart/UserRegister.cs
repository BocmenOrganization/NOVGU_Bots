using BotsCore.User.Models;
using System;
using static NOVGUBots.Moduls.NOVGU_SiteData.Parser;

namespace NOVGUBots.App.NOVGU_Standart
{
    public static class UserRegister
    {
        private const string NameFiledRegisterInfo = "StateRegister";
        private const string NameFiledTypeSchedule = "TypeSchedule";
        private const string NameFiledNameInstituteCollege = "NameInstituteCollege";
        private const string NameFiledNameCourse = "NameCourse";
        private const string NameFiledNameGroup = "NameGroup";
        private const string NameFiledIdUser = "IdUserNOVGU";
        private const string NameFiledUserState = "UserState";
        public static RegisterState GetInfoRegisterUser(ModelUser user)
        {
            if (user[NameFiledRegisterInfo] is RegisterState resul)
                return resul;
            else if (user[NameFiledRegisterInfo] is long resLong)
                return (RegisterState)resLong;
            else
                return RegisterState.NewUser;
        }
        public static void SetRegister(RegisterState registerState, ModelUser user) => user[NameFiledRegisterInfo] = registerState;
        public static void AddFlag(RegisterState registerState, ModelUser user) => SetRegister(GetInfoRegisterUser(user) | registerState, user);
        public static void RemoveFlag(RegisterState registerState, ModelUser user) => SetRegister(GetInfoRegisterUser(user) & ~registerState, user);
        public static TypePars GetTypeSchedule(ModelUser user)
        {
            if (user[NameFiledTypeSchedule] is TypePars resul)
                return resul;
            else if (user[NameFiledTypeSchedule] is long resLong)
                return (TypePars)resLong;
            else
                return TypePars.InstituteFullTime;
        }
        public static void SetTypeSchedule(TypePars type, ModelUser user) => user[NameFiledTypeSchedule] = type;
        public static string GetNameInstituteCollege(ModelUser user) => (string)user[NameFiledNameInstituteCollege];
        public static void SetNameInstituteCollege(string name, ModelUser user) => user[NameFiledNameInstituteCollege] = name;
        public static string GetNameCourse(ModelUser user) => (string)user[NameFiledNameCourse];
        public static void SetNameCourse(string name, ModelUser user) => user[NameFiledNameCourse] = name;
        public static string GetNameGroup(ModelUser user) => (string)user[NameFiledNameGroup];
        public static void SetNameGroup(string name, ModelUser user) => user[NameFiledNameGroup] = name;
        public static void SetUser(string id, ModelUser user) => user[NameFiledIdUser] = id;
        public static string GetUser(ModelUser user) => (string)user[NameFiledIdUser];
        public static void SetUserState(UserState userState, ModelUser user) => user[NameFiledUserState] = userState;
        public static UserState GetUserState(ModelUser user)
        {
            if (user[NameFiledUserState] is UserState resul)
                return resul;
            else if (user[NameFiledUserState] is long resLong)
                return (UserState)resLong;
            else
                return UserState.Student;
        }
        [Flags]
        public enum RegisterState : uint
        {
            NewUser = 1,
            GroupSet = 2,
            LoginPasswordSet = 4,
            ConnectNovgu = 8
        }
        public enum UserState
        {
            Student,
            Teacher
        }
    }
}
