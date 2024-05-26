using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Common.Models
{
    public class HealthCheckInfo : TableEntity
    {
        public Guid Id { get; set; }
        public string Message { get; set; }

        public HealthCheckInfo()
        {
        }

        public HealthCheckInfo(Guid id)
        {
            PartitionKey = nameof(HealthCheckInfo); RowKey = id.ToString();
        }
    }
}
