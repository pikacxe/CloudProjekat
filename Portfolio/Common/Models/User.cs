using Microsoft.WindowsAzure.Storage.Table;

namespace Common.Models
{
    public class User : TableEntity
    {

        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;

        public User()
        {

        }
        public User(string email)
        {
            PartitionKey = nameof(User); RowKey = email;
        }
    }
}
