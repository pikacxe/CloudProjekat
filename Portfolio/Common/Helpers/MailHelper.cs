using System.Threading.Tasks;
using System.Net.Mail;
using Common.Models;
using System.Net;
using System.Collections.Generic;

namespace NotificationService.Helpers
{
    public static class MailHelper
    {
        // # TODO Send mails for each notification


        private static SmtpClient Connect()
        {
            return new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("a59253fc1f3996", "2612348feba4a3"),
                EnableSsl = true
            };
        }

        public static async Task SendAlarmTriggered(ProfitAlarm alarm)
        {
            using(var client = Connect())
            {
                await client.SendMailAsync("info@portfolioservice.com", alarm.UserEmail, "Profit margin alarm triggered",
                    $"Cryptocurrency {alarm.CryptoCurrencyName} exceeded profit margin of {alarm.ProfitMargin}");
            }
        }
        public static async Task SendServiceDown(List<string> adminEmails, string serviceName)
        {
            using (var client = Connect())
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("service-down@portofilioservice.com");
                mailMessage.Subject = $"!!!!! {serviceName} is currently down !!!!!!";
                mailMessage.Body = $"{serviceName} is currently down";
                foreach(string email in adminEmails)
                {
                    mailMessage.To.Add(new MailAddress(email));
                }
                await client.SendMailAsync(mailMessage);
            }
        }

    }
}
