using BotsCore.User.Models;
using System;

namespace NOVGUBots.App.NOVGU_Standart
{
    public static class UserRegister
    {
        private const string NameFiled = "StateRegister";
        public static RegisterState GetInfoRegisterUser(ModelUser user)
        {
            if (user[NameFiled] is RegisterState resul)
                return resul;
            else if (user[NameFiled] is long resLong)
                return (RegisterState)resLong;
            else
                return RegisterState.NewUser;
        }
        public static void SetRegister(RegisterState registerState, ModelUser user) => user[NameFiled] = registerState;
        public static void AddFlag(RegisterState registerState, ModelUser user) => SetRegister(GetInfoRegisterUser(user) | registerState, user);
        [Flags]
        public enum RegisterState : uint
        {
            NewUser = 0,
            GroupSet = 2,
            LoginPasswordSet = 4,
            ConnectNovgu = 8
        }
    }
}
