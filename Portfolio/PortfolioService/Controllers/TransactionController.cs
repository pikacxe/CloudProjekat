using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

using Common.DTOs;
using Common.Models;
using Common.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace PortfolioService.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ICloudRepository<Transaction> _cloudCryptoRepository = new CloudRepository<Transaction>("TransactionTable");
        private readonly ICloudRepository<UserPortfolioEntry> _cloudUserEntryRepository = new CloudRepository<UserPortfolioEntry>("PortfolioEntry");

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

        public ActionResult DeleteTransaction()
        {
            return View("DeleteTransaction");
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
                    await ProccessNewTransaction(transaction);
                    await _cloudCryptoRepository.Add(transaction);
                }
            }
            catch
            {
                return View("Error");
            }
            return RedirectToAction("Index");
        }

        private async Task ProccessNewTransaction(Transaction transaction)
        {
            var existingEntry = await _cloudUserEntryRepository.Get(x => x.PartitionKey == Session["LoggedInUserEmail"].ToString() && x.RowKey==transaction.CryptoName);
            if(existingEntry == null)
            {
                if(transaction.TransactionType == "Sale")
                {
                    throw new ArgumentException("First transaction must be a 'Purchase'");
                }
                var test = new UserPortfolioEntry(transaction.CryptoName, transaction.UserEmail, transaction.Amount, transaction.Price);
                await _cloudUserEntryRepository.Add(test);
                return;
            }
            else
            {
                if(transaction.TransactionType == "Sale")
                {
                    existingEntry.UpdateOnSale(transaction.Amount, transaction.Price);
                }
                else if(transaction.TransactionType == "Purchase")
                {
                    existingEntry.UpdateOnPurchase(transaction.Amount, transaction.Price);
                }
                else
                {
                    throw new ArgumentException($"Invalid transaction type: '{transaction.TransactionType}'");
                }
                await _cloudUserEntryRepository.Update(existingEntry);
            }
        }
        [HttpPost]
        public async Task<ActionResult> DeleteTransaction(Guid transactionId)
        {
            try
            {
                // Check if is loggedIn
                if (Session["LoggedInUserEmail"] != null)
                {
                    await _cloudCryptoRepository.Delete(transactionId.ToString());
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