using BotsCore.Moduls.Tables.Services;
using BotsCore.Moduls.Translate;
using System.Net;
using System.Net.Mail;

namespace NOVGUBots.App.NOVGU_Standart.Pages.Auntification.NOVGUAuntification
{
    public static class AuthorizationMailing
    {
        private static readonly ModelMarkerTextData TitleText = new(CreatePageAppStandart.NameApp, CreatePageAppStandart.NameTableText, 62);
        private static readonly ModelMarkerTextData TextMessage = TitleText.GetElemNewId(63);
        private static (string email, string password)[] clientsInfo;
        private static string adressSMPT;
        private static int portSMPT;
        private static int Id = 0;
        private static object LockObj = new();
        public static void Start(string adressSMPT, int portSMPT, (string email, string password)[] clientsInfo) => (AuthorizationMailing.adressSMPT, AuthorizationMailing.portSMPT, AuthorizationMailing.clientsInfo) = (adressSMPT, portSMPT, clientsInfo);
        public static void Send(string codeSend, string idSend, string email, string userName, Lang.LangTypes lang)
        {
            (SmtpClient smtpClient, MailAddress from) = GetClient();
            MailMessage mailMessage = new MailMessage(from, new MailAddress(email, userName));
            mailMessage.Subject = TitleText.GetText(lang);
            mailMessage.Body = string.Format(TextMessage.GetText(lang), codeSend);
            mailMessage.IsBodyHtml = true;
            smtpClient.Send(mailMessage);
        }
        private static (SmtpClient, MailAddress) GetClient()
        {
            lock (LockObj)
            {
                Id++;
                if (Id >= clientsInfo.Length)
                    Id = 0;
                SmtpClient smtpClient = new SmtpClient(adressSMPT, portSMPT);
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(clientsInfo[Id].email, clientsInfo[Id].password); ;
                return (smtpClient, new MailAddress(clientsInfo[Id].email));
            }
        }
    }
}
