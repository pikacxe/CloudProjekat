using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
