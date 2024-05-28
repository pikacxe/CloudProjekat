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
        public DateTime DateCreated { get; set; }

        public ProfitAlarm() { }

        public ProfitAlarm(string userEmail)
        {
            ProfitAlarmId = Guid.NewGuid();
            UserEmail = userEmail;
            PartitionKey = userEmail;
            RowKey = ProfitAlarmId.ToString();
        }
    }
}
