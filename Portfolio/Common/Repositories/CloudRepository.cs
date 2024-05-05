using Common.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public class CloudRepository<T> : ICloudRepository<T> where T : TableEntity, new()
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public CloudRepository(string tableName)
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference(tableName); _table.CreateIfNotExistsAsync();
        }

        public async Task Add(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            TableOperation insertOperation = TableOperation.Insert(entity);
            await _table.ExecuteAsync(insertOperation);
        }

        public async Task Delete(string rowKey)
        {
            var entity = await Get(rowKey);
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            TableOperation deleteOperation = TableOperation.Delete(entity);
            await _table.ExecuteAsync(deleteOperation);
        }

        public async Task<T> Get(string rowKey)
        {
            return await Get(x => x.RowKey == rowKey);
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter)
        {
            var res = await GetAll(filter);
            var entity = res.FirstOrDefault();
            return entity;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await GetAll(x => x.PartitionKey == typeof(T).Name);
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter)
        {
            var query = _table.CreateQuery<T>().Where(filter);
            TableContinuationToken token = new TableContinuationToken();
            var results = await _table.ExecuteQuerySegmentedAsync(query.AsTableQuery(),token);
            return results.Results;
        }

        public async Task Update(T entity)
        {
            TableOperation updateOperation = TableOperation.Replace(entity);
            await _table.ExecuteAsync(updateOperation);
        }
    }
}
