using Common.Models;
using Common.Repositories;
using System.Web.Mvc;

namespace PortfolioService.Controllers
{
    public class UserController : Controller
    {
        UserRepository repo = new UserRepository();

        // GET: User
        public ActionResult Index()
        {
            return View(repo.RetrieveAllUsers());
        }

        public ActionResult Create()
        {
            User newUser = new User();
            return View("AddEntity", newUser);
        }

        public ActionResult Delete(string id)
        {
            repo.RemoveUser(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddEntity(User receivedUser)
        {
            try
            {
                if (repo.Exists(receivedUser.Email))
                {
                    return View("Error");
                }

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

                repo.AddUser(user);
                return RedirectToAction("Index");
            }
            catch
            {
                return View("AddEntity");
            }
        }
    }
}