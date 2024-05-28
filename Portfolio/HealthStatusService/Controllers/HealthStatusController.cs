using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common.Models;
using Common.Repositories;

namespace HealthStatusService.Controllers
{
    public class HealthStatusController : Controller
    {
        ICloudRepository<HealthCheckInfo> _healthCheckRepository = new CloudRepository<HealthCheckInfo>("HealthCheck");

        public async Task<ActionResult> Index()
        {
            var statuses = await _healthCheckRepository.GetAll();

            var last24Hours = DateTime.UtcNow.AddHours(-24);
            statuses = statuses.Where(s => ExtractTimestamp(s.Message) >= last24Hours);

            var portfolioMessages = statuses.Where(s => s.Message.Contains("PORTFOLIO"));
            var notificationMessages = statuses.Where(s => s.Message.Contains("NOTIFICATION"));

            int portfolioOkCount = portfolioMessages.Count(s => s.Message.Contains("PORTFOLIO_OK"));
            int portfolioNotOkCount = portfolioMessages.Count(s => s.Message.Contains("PORTFOLIO_NOT_OK"));

            int notificationOkCount = notificationMessages.Count(s => s.Message.Contains("NOTIFICATION_OK"));
            int notificationNotOkCount = notificationMessages.Count(s => s.Message.Contains("NOTIFICATION_NOT_OK"));

            double portfolioUptimePercentage = CalculateUptimePercentage(portfolioOkCount, portfolioNotOkCount);
            double notificationUptimePercentage = CalculateUptimePercentage(notificationOkCount, notificationNotOkCount);

            ViewBag.PortfolioUptime = portfolioUptimePercentage;
            ViewBag.NotificationUptime = notificationUptimePercentage;

            ViewBag.PortfolioOkCount = portfolioOkCount;
            ViewBag.PortfolioNotOkCount = portfolioNotOkCount;

            ViewBag.NotificationOkCount = notificationOkCount;
            ViewBag.NotificationNotOkCount = notificationNotOkCount;

            return View();
        }

        private DateTime ExtractTimestamp(string message)
        {
            var timestampString = message.Split(' ')[1].Replace("_PORTFOLIO_OK", "").Replace("_PORTFOLIO_NOT_OK", "").Replace("_NOTIFICATION_OK", "").Replace("_NOTIFICATION_NOT_OK", "");
            return DateTime.Parse(timestampString);
        }

        private double CalculateUptimePercentage(int okCount, int notOkCount)
        {
            int totalCount = okCount + notOkCount;
            if (totalCount == 0) return 0;
            return (double)okCount / totalCount * 100;
        }
    }
}