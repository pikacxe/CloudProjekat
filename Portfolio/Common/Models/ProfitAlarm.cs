using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Common.Models
{
    public class ProfitAlarm:TableEntity
    {
        public Guid ProfitAlarmId { get; set; } 
        public string UserEmail { get; set; } = string.Empty;
        public string CryptoCurrencyName { get; set; } = string.Empty;
        public double ProfitMargin { get; set; }
        public Guid TransactionId { get; set; }

        public DateTime DateCreated { get; set; }

        public ProfitAlarm() { }

        public ProfitAlarm(Guid profitAlarmId, Guid transactionId)
        {
            PartitionKey = nameof(ProfitAlarm);
            RowKey = profitAlarmId.ToString();
            TransactionId = transactionId;
        }

        public override string ToString()
        {
            // TODO
            return $"";
        }
    }
}
