using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Table;



namespace Common.Models
{
    public class Transaction:TableEntity
    {
        public Transaction(string cryptoName,Guid transacionId)
        {
            PartitionKey = cryptoName;
            RowKey = transacionId.ToString();
            TransactionId= transacionId;
        }
        public Transaction()
        {
        }

        public Guid TransactionId { get; set; }
        public string CryptoName { get; set; } = string.Empty;
        public double Amount { get; set; } = 0;
        public double Price { get; set; } = 0;
        public DateTime Date { get; set; }
        public string UserEmail {  get; set; }
     
        public string TransactionType { get; set; }



    }
}
