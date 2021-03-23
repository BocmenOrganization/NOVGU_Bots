using BotsCore.Moduls.Tables.Services;
using BotsCore.Moduls.Translate;
using System.Net.Mail;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification
{
    public static class AuthorizationMailing
    {
        private static readonly ModelMarkerTextData Message_TextSend = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 39);
        private static (SmtpClient, MailAddress)[] smtpClients;
        private static int Id = 0;
        private static object LockObj = new object();
        public static void Start((SmtpClient, MailAddress)[] smtpClients) => AuthorizationMailing.smtpClients = smtpClients;
        public static void Send(string codeSend, string idSend, string email, string userName, Lang.LangTypes lang)
        {
            (SmtpClient smtpClient, MailAddress from) = GetClient();
            MailMessage mailMessage = new MailMessage(from, new MailAddress(email, userName));
            mailMessage.Subject = "Мудрец - подтверждение авторизации";
            mailMessage.Body = codeSend;
            mailMessage.IsBodyHtml = true;
            smtpClient.Send(mailMessage);
        }
        private static (SmtpClient, MailAddress) GetClient()
        {
            lock (LockObj)
            {
                Id++;
                if (Id >= smtpClients.Length)
                    Id = 0;
                return smtpClients[Id];
            }
        }
    }
}
