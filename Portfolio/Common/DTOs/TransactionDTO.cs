using System;

namespace Common.DTOs
{
    public class TransactionDTO
    {
        public string CryptoName { get; set; } = string.Empty;
        public double Amount { get; set; } = 0;
        public double Price { get; set; } = 0;
        public DateTime Date { get; set; }
        public string TransactionType { get; set; }  
    }
}
