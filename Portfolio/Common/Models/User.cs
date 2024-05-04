using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Common.Models
{
    public class User : TableEntity
    {

        public String Name { get; set; } = string.Empty;
        public String LastName { get; set; } = string.Empty;
        public String Address { get; set; } = string.Empty;
        public String City { get; set; } = string.Empty;
        public String Country { get; set; } = string.Empty;
        public String PhoneNumber { get; set; } = string.Empty;
        public String Email { get; set; } = string.Empty;
        public String Password { get; set; } = string.Empty;
        public String Picture { get; set; } = string.Empty;

        public User()
        {

        }
        public User(String email)
        {
            PartitionKey = nameof(User); RowKey = email;
        }
    }
}
