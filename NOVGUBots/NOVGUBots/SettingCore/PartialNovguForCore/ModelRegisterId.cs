namespace NOVGUBots.User.Models
{
    public partial class ModelRegisterId
    {
        public RegisterState registerState = RegisterState.No;
        public bool IsRegister => !((registerState & RegisterState.No) > 0);
        public enum RegisterState
        {
            No = 1,
            GroupSet = 2,
            LoginPasswordSet = 4,
            ConnectNovgu = 8
        }
    }
}
