using Common.Models;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IAdminConsole
    {
        [OperationContract]
        string Authenticate(string username, string password);
        [OperationContract]
        Task<IEnumerable<User>> ListUsersAsync(string adminKey);
        [OperationContract]
        Task<string> DeleteByIdAsync(string adminKey, string userEmail);

        [OperationContract]
        string UpdateEmail(string adminKey, string email, string updatedEmail = "");

        [OperationContract]
        IEnumerable<string> ListAdminEmails(string adminKey);
    }
}
