using Common.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;

namespace Common.Repositories
{
    public class UserRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public UserRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("UserTable"); _table.CreateIfNotExists();
        }

        public IQueryable<User> RetrieveAllUsers()
        {
            var results = from g in _table.CreateQuery<User>()
                          where g.PartitionKey == "User"
                          select g;
            return results;
        }
        public void AddUser(User newUser)
        { // Samostalni rad: izmestiti tableName u konfiguraciju servisa. 
            TableOperation insertOperation = TableOperation.Insert(newUser);
            _table.Execute(insertOperation);
        }

        public bool Exists(string email)
        {
            return RetrieveAllUsers().Where(u => u.RowKey == email).FirstOrDefault() != null;
        }

        public void RemoveUser(string email)
        {
            User user = RetrieveAllUsers().Where(s => s.RowKey == email).FirstOrDefault();

            if (user != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(user);
                _table.Execute(deleteOperation);
            }
        }

        public User GetUser(string email)
        {
            return RetrieveAllUsers().Where(p => p.RowKey == email).FirstOrDefault();
        }

        public void UpdateStudent(User user)
        {
            TableOperation updateOperation = TableOperation.Replace(user);
            _table.Execute(updateOperation);
        }
    }
}
