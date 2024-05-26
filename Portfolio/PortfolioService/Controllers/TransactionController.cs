using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

using Common.DTOs;
using Common.Models;
using Common.Repositories;

namespace PortfolioService.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ICloudRepository<Transaction> _cloudCryptoRepository = new CloudRepository<Transaction>("TransactionTable");
        private readonly ICloudRepository<ProfitAlarm> _activeAlarmRepo = new CloudRepository<ProfitAlarm>("ActiveAlarmsTable");

        public async Task<ActionResult> Index()
        {
            if (Session["LoggedInUserEmail"] == null)
            {
                return RedirectToAction("Login");
            }

            string email = Session["LoggedInUserEmail"].ToString();
            var transaction = await _cloudCryptoRepository.GetAll(c => c.UserEmail == email);
            return View(transaction);
        }

        public ActionResult AddTransaction()
        {
            return View("AddTransaction");
        }
        


        [HttpPost]
        public async Task<ActionResult> AddTransaction(TransactionDTO recivedTransaction)
        {
            try
            {
                // Check if is loggedIn
                if (Session["LoggedInUserEmail"] != null)
                {
                    Transaction transaction = new Transaction(recivedTransaction.CryptoName, Guid.NewGuid())
                    {
                        CryptoName = recivedTransaction.CryptoName,
                        Amount = recivedTransaction.Amount,
                        Price = recivedTransaction.Price,
                        Date = recivedTransaction.Date,
                        UserEmail = Session["LoggedInUserEmail"].ToString(),
                        TransactionType = recivedTransaction.TransactionType
                    };
                    ProfitAlarm profitAlarm = new ProfitAlarm(Guid.NewGuid(),transaction.TransactionId)
                    {
                        CryptoCurrencyName = transaction.CryptoName,
                        UserEmail = transaction.UserEmail,
                        DateCreated = transaction.Date,
                        ProfitMargin = recivedTransaction.Profit

                    };
                    await _cloudCryptoRepository.Add(transaction);
                    await _activeAlarmRepo.Add(profitAlarm);
                }
            }
            catch
            {
                return View("Error");
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> CryptoInfo()
        {
            string email = Session["LoggedInUserEmail"].ToString();
            List<Transaction> transactions = await _cloudCryptoRepository.GetAll(c => c.UserEmail == email) as List<Transaction>;
            CryptoInfo ci = new CryptoInfo();
            await ci.PopulateInfo(transactions);
            return View(ci);
        }

    }

}