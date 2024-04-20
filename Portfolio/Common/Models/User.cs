using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Common.Models
{
    public class User : TableEntity
    {

        public String Name { get; set; } = "";
        public String LastName { get; set; } = "";
        public String Address { get; set; } = "";
        public String City { get; set; } = "";
        public String Country { get; set; } = "";
        public String PhoneNumber { get; set; } = "";
        public String Email { get; set; } = "";
        public String Password { get; set; } = "";
        public String Picture { get; set; } = "";

        public User()
        {

        }
        public User(String email)
        {
            PartitionKey = "User"; RowKey = email;
        }
    }
}
