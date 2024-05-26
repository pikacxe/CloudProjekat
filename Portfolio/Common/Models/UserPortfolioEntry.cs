using Microsoft.WindowsAzure.Storage.Table;

namespace Common.Models
{
    public class UserPortfolioEntry : TableEntity
    {
        public string CryptoName { get; private set; } = string.Empty;
        public string UserEmail { get; private set; } = string.Empty ;
        public double Amount { get; private set; }
        public double PaidPrice { get; set; }
        public double CurrentProfit { get; set; }
        public double CurrentValue { get; set; }
        public UserPortfolioEntry()
        {
            PartitionKey = UserEmail;
            RowKey = CryptoName;
        }

        public UserPortfolioEntry(string cryptoName, string userEmail, double amount, double paidPrice)
        {
            PartitionKey = UserEmail;
            RowKey = CryptoName;
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
