using Common.Models;
using Common.Repositories;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PortfolioService.Controllers
{
    public class UserController : Controller
    {
        ICloudRepository<User> _cloudRepository = new CloudRepository<User>("UserTable");

        // GET: User
        public async Task<ActionResult> Index()
        {
            return View(await _cloudRepository.GetAll());
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

        public ActionResult Create()
        {
            return View("AddEntity");
        }

        [HttpPost]
        public async Task<ActionResult> Create(User receivedUser)
        {
            try
            {
                // Check if exists
                if (await _cloudRepository.Get(receivedUser.RowKey) != null)
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
            return RedirectToAction("Index");
        }
    }
}