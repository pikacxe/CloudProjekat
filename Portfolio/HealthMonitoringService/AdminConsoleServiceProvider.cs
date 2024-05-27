using Common;
using Common.Models;
using Common.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthMonitoringService
{
    public class AdminConsoleServiceProvider : IAdminConsole
    {
        private readonly ICloudRepository<User> _userRepo = new CloudRepository<User>("UserTable");
        private List<string> _activeAdmins = new List<string>();
        private Dictionary<string, string> _adminAccounts = new Dictionary<string, string>();

        public AdminConsoleServiceProvider()
        {
            _adminAccounts.Add("admin", "admin");
        }

        public string Authenticate(string username, string password)
        {
            if(_adminAccounts.ContainsKey(username))
            {
                if (_adminAccounts[username] == password)
                {
                    Guid adminKey = Guid.NewGuid();
                    _activeAdmins.Add(adminKey.ToString());
                    return adminKey.ToString();
                }
            }
            return string.Empty;
        }

        public async Task<string> DeleteByIdAsync(string adminKey, string email)
        {
            if (_activeAdmins.Contains(adminKey))
            {
                await _userRepo.Delete(email);
                return "User deleted successfully";
            }
            else
            {
                return "Error while deleting user";
            }
        }

        public async Task<IEnumerable<User>> ListUsersAsync(string adminKey)
        {
            if (_activeAdmins.Contains(adminKey))
            {
                return await _userRepo.GetAll();
            }
            return Enumerable.Empty<User>();
        }
    }
}