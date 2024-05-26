using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common.Models;

using Common.Helpers;
using System.Runtime.InteropServices;

namespace Common.DTOs
{
    public class CryptoInfo
    {
       public Dictionary<string,CryptoDto> Cryptos=new Dictionary<string,CryptoDto>();
       public double TotalValue;
       public double TotalProfit;

        public CryptoInfo()
        {
            
        }
        public async Task PopulateInfo(List<Transaction> transactions)
        {
            foreach (Transaction transaction in transactions)
            {
                double price = await CryptoInformationHelper.CheckPrice(transaction.CryptoName);
                if (Cryptos.ContainsKey(transaction.CryptoName))
                {
                    var cryptoCurrent = Cryptos[transaction.CryptoName];
                    cryptoCurrent.Profit += transaction.TransactionType.Equals("Purchase") ? -transaction.Price : transaction.Price;
                    cryptoCurrent.Amount += transaction.TransactionType.Equals("Purchase") ? -transaction.Amount : transaction.Amount;
                    cryptoCurrent.Value = price* cryptoCurrent.Amount;
                    //TO DO dodaj crypto u tabelu
                    TotalValue += cryptoCurrent.Value;
                    TotalProfit+= cryptoCurrent.Profit;
                }
                else
                {
                    CryptoDto c = new CryptoDto()
                    {
                        Name = transaction.CryptoName,
                        Amount = transaction.Amount,
                        Value = price*transaction.Amount
                    };
                    TotalValue += c.Value;
                    Cryptos.Add(c.Name, c);
                }
            }
            
        }

    }
    public class CryptoDto
    {
        public string Name { get; set; } = string.Empty;
        public double Profit { get; set; }
        public double Value {  get; set; }
        public double Amount { get; set; }

        public CryptoDto()
        {
         
        }

    }
}
