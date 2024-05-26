using System.Threading.Tasks;
using System.Net.Mail;
using Common.Models;
using System.Net;

namespace NotificationService.Helpers
{
    public static class MailHelper
    {
        // # TODO Send mails for each notification


        private static SmtpClient Connect()
        {
            return new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("a0ea78ef0ee92f", "caa09a7c5e7c96"),
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
        public static async Task SendServiceDown(string email, string serviceName)
        {
            using (var client = Connect())
            {
                await client.SendMailAsync("admin@portfolioservice.com", email, $"!!!! {serviceName} is DOWN!!!!",
                $"It seems that {serviceName} is currently down!!!");
            }
        }

    }
}
