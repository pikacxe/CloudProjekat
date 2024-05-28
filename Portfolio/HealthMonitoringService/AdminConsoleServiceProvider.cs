using Common;
using Common.Models;
using Common.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HealthMonitoringService
{
    public class AdminConsoleServiceProvider : IAdminConsole
    {
        private readonly ICloudRepository<User> _userRepo = new CloudRepository<User>("UserTable");
        private List<string> _activeAdmins = new List<string>();
        private Dictionary<string, string> _adminAccounts = new Dictionary<string, string>();
        public static List<string> adminEmails = new List<string>
        {
            "admin@admin.com"
        };
        private readonly Regex reg = new Regex(@"^[a-zA-z0-9]+[\-\.]?[a-zA-z0-9]+@[a-zA-Z0-9]+\.[a-zA-Z0-9]+$");
        public AdminConsoleServiceProvider()
        {
            _adminAccounts.Add("admin", "admin");
        }

        public string Authenticate(string username, string password)
        {
            if (_adminAccounts.ContainsKey(username))
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
                return "Unauthorized";
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

        public IEnumerable<string> ListAdminEmails(string adminKey)
        {
            if (_activeAdmins.Contains(adminKey))
            {
                return adminEmails;
            }
            else
            {
                return new List<string>
                {
                    "Unauthorized"
                };
            }
        }

        public string UpdateEmail(string adminKey, string email, string updateEmail = "")
        {
            if (!reg.IsMatch(email))
            {
                return "Invalid email format";
            }
            if (_activeAdmins.Contains(adminKey))
            {
                if (!adminEmails.Contains(email))
                {
                    adminEmails.Add(email);
                    return "Email added successfully";
                }
                else
                {
                    if (updateEmail == "")
                    {
                        return "Email you want to update does not exist";
                    }
                    else
                    {
                        if (!reg.IsMatch(updateEmail))
                        {
                            return "Invalid format";
                        }
                        adminEmails.Remove(email);
                        adminEmails.Add(updateEmail);
                        return "Admin email updated successfully";
                    }
                }
            }
            return "Unauthorized";
        }
    }
}