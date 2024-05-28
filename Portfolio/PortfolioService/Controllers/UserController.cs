using Common.DTOs;
using Common.Helpers;
using Common.Models;
using Common.Repositories;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace PortfolioService.Controllers
{
    public class UserController : Controller
    {
        ICloudRepository<User> _cloudRepository = new CloudRepository<User>("UserTable");
        ICloudRepository<UserPortfolioEntry> _userEntriesRepository = new CloudRepository<UserPortfolioEntry>("PortfolioEntry");
        ICloudRepository<ProfitAlarm> _alarmRepo = new CloudRepository<ProfitAlarm>("ActiveAlarmsTable");
        ICloudRepository<ProfitAlarm> _doneAlarmRepo = new CloudRepository<ProfitAlarm>("DoneAlarmsTable");
        public static List<string> ids = new List<string>();
        public async Task<ActionResult> Index()
        {
            var email = Session["LoggedInUserEmail"].ToString();
            if (email == null)
            {
                return View("Login");
            }
            var entries = await _userEntriesRepository.GetAll(x => x.PartitionKey == email);
            UserPortfolio up = new UserPortfolio(entries);
            await up.CalculateTotals();
            return View("Index", up);
        }


        public async Task<ActionResult> AlarmsView()
        {
            try
            {

                CloudQueue queue = QueueHelper.GetQueueReference("alarmsqueue");
                CloudQueueMessage message = queue.GetMessage();
                if (message == null)
                {
                    Trace.TraceInformation("No messages in queue.", "Information");
                }
                else
                {
                    Trace.TraceInformation($"Queue message: {message.AsString}");
                    string doneAlarms = message.AsString;
                    ids = doneAlarms.TrimStart('|').Split('|').ToList();
                    queue.DeleteMessage(message);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
            string email = Session["LoggedInUserEmail"].ToString();
            IEnumerable<ProfitAlarm> alarms = await _doneAlarmRepo.GetAll(x => x.PartitionKey == email);
            return View("AlarmsView", alarms);
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
        //public ActionResult DeleteUserPortfolioEntry()
        //{
        //    return View("DeleteUserPortfolioEntry");
        //}


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
        public async Task<ActionResult> Register(UserDTO receivedUser, HttpPostedFileBase file)
        {
            try
            {
                // Check if exists
                if (await _cloudRepository.Get(receivedUser.Email) == null)
                {
                    string uniqueBlobName = string.Format("image_{0}", receivedUser.Email);
                    var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                    var blobStorage = storageAccount.CreateCloudBlobClient();
                    var container = blobStorage.GetContainerReference("vezba");
                    await container.CreateIfNotExistsAsync();
                    await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                    var blob = container.GetBlockBlobReference(uniqueBlobName);
                    blob.Properties.ContentType = file.ContentType;
                    using (var fileStream = file.InputStream)
                    {
                        await blob.UploadFromStreamAsync(fileStream);
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
                        Picture = blob.Uri.ToString()
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
            if (existingUser != null && existingUser.Password == password)
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
                    // existingUser.Picture = receivedUser.Picture;


                    await _cloudRepository.Update(existingUser);
                }
            }
            catch
            {
                return View("Error");
            }
            return RedirectToAction("UpdateProfile");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteUserPortfolioEntry(string cryptoName)
        {
            try
            {
                // Check if is loggedIn
                if (Session["LoggedInUserEmail"] != null)
                {
                    await _userEntriesRepository.Delete(cryptoName);
                }
                else
                {
                    return View("Login");
                }
            }
            catch
            {
                return View("Error");
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<ActionResult> AddAlarmForCrypto(string cryptoName, int cryptoMargin)
        {
            try
            {
                // Check if is loggedIn
                if (Session["LoggedInUserEmail"] != null)
                {
                    ProfitAlarm pa = new ProfitAlarm(Session["LoggedInUserEmail"].ToString());
                    pa.CryptoCurrencyName = cryptoName;
                    pa.ProfitMargin = cryptoMargin;
                    pa.DateCreated = DateTime.Now;
                    await _alarmRepo.Add(pa);
                    string email = Session["LoggedInUserEmail"].ToString();
                    IEnumerable<ProfitAlarm> alarms = await _doneAlarmRepo.GetAll(x => x.PartitionKey == email);
                    return RedirectToAction("AlarmsView", alarms);
                }
                else
                {
                    return View("Login");
                }
            }
            catch
            {
                return View("Error");
            }
        }
    }
}