using System;

namespace Common.DTOs
{
    public class TransactionDTO
    {
        public Guid TransactionID { get; set; }
        public string CryptoName { get; set; } = string.Empty;
        public double Amount { get; set; } = 0;
        public double Price { get; set; } = 0;
        public double Profit { get; set; } = 0;
        public DateTime Date { get; set; }
        public string TransactionType { get; set; }  
    }
}
