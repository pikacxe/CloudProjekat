using Common.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Models
{
    public class UserPortfolio
    {
        public double TotalProfit { get; private set; }
        public double TotalValue { get; private set; }
        public IEnumerable<UserPortfolioEntry> Entries { get; private set; }

        public UserPortfolio()
        {
            Entries = new List<UserPortfolioEntry>();
        }

        public UserPortfolio(IEnumerable<UserPortfolioEntry> entries)
        {
            Entries = entries;
        }

        public async Task CalculateTotals()
        {
            Random r = new Random();
            foreach (var entry in Entries)
            {
                //double price = await CryptoInformationHelper.CheckPrice(entry.CryptoName);
                double price = await Task.FromResult(r.NextDouble()*100000); // for development
                entry.CurrentValue = price * entry.Amount;
                TotalValue += entry.CurrentValue;
            }
        }
    }
}
