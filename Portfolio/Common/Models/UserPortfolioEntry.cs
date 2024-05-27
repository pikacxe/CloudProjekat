using Microsoft.WindowsAzure.Storage.Table;

namespace Common.Models
{
    public class UserPortfolioEntry : TableEntity
    {
        public string CryptoName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty ;
        public double Amount { get; set; }
        public double PaidPrice { get; set; }
        public double CurrentProfit { get; set; }
        public double CurrentValue { get; set; }

        public UserPortfolioEntry() { }
        public UserPortfolioEntry(string cryptoName, string userEmail, double amount, double paidPrice)
        {
            PartitionKey = userEmail;
            RowKey = cryptoName;
            CryptoName = cryptoName;
            UserEmail = userEmail;
            Amount = amount;
            PaidPrice = paidPrice;
            CurrentProfit = -paidPrice;
        }

        public void UpdateOnSale(double amount, double soldFor)
        {
            Amount -= amount;
            CurrentProfit += soldFor;
            PaidPrice -= soldFor;
        }

        public void UpdateOnPurchase(double amount, double price)
        {
            Amount += amount;
            PaidPrice += price;
            CurrentProfit -= price;
        }
    }
}
