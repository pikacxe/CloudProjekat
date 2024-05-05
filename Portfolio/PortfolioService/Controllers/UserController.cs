using Common.DTOs;
using Common.Models;
using Common.Repositories;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PortfolioService.Controllers
{
    public class UserController : Controller
    {
        ICloudRepository<User> _cloudRepository = new CloudRepository<User>("UserTable");

        public ActionResult Index()
        {
            return View("Index");
        }

        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                await _cloudRepository.Delete(id);
            }
            catch
            {
                return View("Error");
            }
            return RedirectToAction("Index");
        }

        public ActionResult Register()
        {
            return View("Register");
        }

        public ActionResult LogIn()
        {
            return View("Login");
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            return View("LogIn");
        }

        public async Task<ActionResult> UpdateProfile()
        {
            string loggedInUserEmail = Session["LoggedInUserEmail"].ToString(); 
            var existingUser = await _cloudRepository.Get(loggedInUserEmail);

            if (existingUser != null)
            {
                var userDto = new UserDTO
                {
                    Name = existingUser.Name,
                    LastName = existingUser.LastName,
                    Address = existingUser.Address,
                    City = existingUser.City,
                    Country = existingUser.Country,
                    PhoneNumber = existingUser.PhoneNumber,
                    Email = existingUser.Email,
                    Password = existingUser.Password,
                    Picture = existingUser.Picture
                };
                return View("UpdateProfile", userDto);
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Register(UserDTO receivedUser)
        {
            try
            {
                // Check if exists
                if (await _cloudRepository.Get(receivedUser.Email) == null)
                {
                    User user = new User(receivedUser.Email)
                    {
                        Name = receivedUser.Name,
                        LastName = receivedUser.LastName,
                        Address = receivedUser.Address,
                        City = receivedUser.City,
                        Country = receivedUser.Country,
                        PhoneNumber = receivedUser.PhoneNumber,
                        Email = receivedUser.Email,
                        Password = receivedUser.Password,
                        Picture = receivedUser.Picture
                    };

                    await _cloudRepository.Add(user);
                }
            }
            catch
            {
                return View("Error");
            }
            return RedirectToAction("LogIn");
        }

        [HttpPost]
        public async Task<ActionResult> LogIn(string email, string password)
        {

            var existingUser = await _cloudRepository.Get(email);
            if(existingUser != null && existingUser.Password == password)
            {
                Session["LoggedInUserEmail"] = existingUser.Email;   
                return RedirectToAction("Index");
            }
 
            return RedirectToAction("LogIn");
        }

        [HttpPost]
        public async Task<ActionResult> UpdateProfile(UserDTO receivedUser)
        {
            try
            {
                var existingUser = await _cloudRepository.Get(receivedUser.Email);
                // Check if exists
                if (existingUser != null)
                {
                    existingUser.Name = receivedUser.Name;
                    existingUser.LastName = receivedUser.LastName;
                    existingUser.Address = receivedUser.Address;
                    existingUser.City = receivedUser.City;
                    existingUser.Country = receivedUser.Country;
                    existingUser.PhoneNumber = receivedUser.PhoneNumber;
                    existingUser.Email = receivedUser.Email;
                    existingUser.RowKey = receivedUser.Email;
                    existingUser.Password = receivedUser.Password;
                    existingUser.Picture = receivedUser.Picture;
                    

                    await _cloudRepository.Update(existingUser);
                }
            }
            catch
            {
                return View("Error");
            }
            return RedirectToAction("UpdateProfile");
        }
    }
}