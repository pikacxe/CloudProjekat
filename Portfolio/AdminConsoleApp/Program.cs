using Common;
using StudentServiceClient.UniversalConnector;
using System;

namespace AdminConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServiceConnector<IAdminConsole> serviceConnector = new ServiceConnector<IAdminConsole>();
                serviceConnector.Connect("net.tcp://localhost:10106/admin-console");
                IAdminConsole proxy = serviceConnector.GetProxy();

                string input = string.Empty;
                Console.Write("Authentication needed!\nEnter username: ");
                string username = Console.ReadLine().Trim();
                Console.Write("Enter password: ");
                string password = Console.ReadLine().Trim();
                string res = proxy.Authenticate(username, password);
                if (res == string.Empty)
                {
                    Console.WriteLine("Invalid username or password");
                    return;
                }
                do
                {
                    Console.WriteLine("Menu\n");
                    Console.WriteLine("1 - List all users");
                    Console.WriteLine("2 - Delete user by email");
                    Console.WriteLine("3 - List all admin emails");
                    Console.WriteLine("4 - Add / Update admin email");
                    Console.WriteLine("q - exit ");

                    input = Console.ReadLine();
                    switch (input)
                    {
                        case "1": ListAllUsers(res, proxy); break;
                        case "2": DeleteUserUsers(res, proxy); break;
                        case "3": ListAllAdminEmails(res, proxy); break;
                        case "4": AddAdminEmail(res, proxy); break;
                    }
                }
                while (input != "q");


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            // Debug purpose
            Console.ReadLine();
        }

        private static void ListAllAdminEmails(string adminKey, IAdminConsole proxy)
        {
            var emails = proxy.ListAdminEmails(adminKey);
            Console.WriteLine("=======================================");
            Console.WriteLine("Admin email");
            foreach (var email in emails)
            {
                Console.WriteLine(string.Format("{0,-30}\n",email));
            }
            Console.WriteLine("=======================================");
        }

        private static void AddAdminEmail(string adminKey, IAdminConsole proxy)
        {
            Console.WriteLine("Add new or update existing? (N - new, U - update, Q - cancel");
            string input;
            do
            {
                input = Console.ReadLine().Trim().ToUpper();
                if (input == "N")
                {
                    Console.WriteLine("Enter new admine email:");
                    string newEmail = Console.ReadLine().Trim();
                    string res = proxy.UpdateEmail(adminKey, newEmail);
                    Console.WriteLine(res);
                    return;
                }
                else if (input == "U")
                {
                    Console.WriteLine("Enter admin email to change:");
                    string existingEmail = Console.ReadLine().Trim();
                    Console.WriteLine("Enter updated email:");
                    string updateEmail = Console.ReadLine().Trim();
                    string res = proxy.UpdateEmail(adminKey, existingEmail, updateEmail);
                    Console.WriteLine(res);
                    return;
                }
                Console.WriteLine("Invalid option");
            } while (input != "q");

        }

        private static async void DeleteUserUsers(string adminKey, IAdminConsole proxy)
        {
            Console.Write("Enter user email to delete:");
            string emailToDelete = Console.ReadLine();
            var res = await proxy.DeleteByIdAsync(adminKey, emailToDelete);
            Console.WriteLine(res);
        }

        private static async void ListAllUsers(string adminKey, IAdminConsole proxy)
        {
            var users = await proxy.ListUsersAsync(adminKey);
            Console.WriteLine("=======================================");
            Console.WriteLine(string.Format("{0,-20}|{1,-15}|{2,-15}|{3,10}\n", "Email", "Name", "Last name", "Phone number"));
            foreach (var user in users)
            {
                Console.WriteLine(string.Format("{0,-20}|{1,-15}|{2,-15}|{3,10}\n", user.Email, user.Name, user.LastName, user.PhoneNumber));
            }
            Console.WriteLine("=======================================");

        }
    }
}
