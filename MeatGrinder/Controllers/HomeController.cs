using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using MeatGrinder.Helpers;

using MeatGrinder.Services;

namespace MeatGrinder.Controllers
{
    using MeatGrinder.DAL.Models;

    public class HomeController : Controller
    {
        [CustomAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize]
        public ActionResult AccountDetails()
        {
            var account = new User
            {
                AccountName = "Meshed",
                EmailAddress = "marksbrown@gmail.com",
            };

            return Json(account, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Login(LoginViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.Password))
            {
                ModelState.AddModelError("", "Password is required!");                
            }
            if (string.IsNullOrEmpty(viewModel.EmailAddress))
            {
                ModelState.AddModelError("", "Email address is required!");
            }
            if (ModelState.IsValid)
            {
                string passwordHash = GetHash(viewModel.Password);
                var db = new MeatGrinderEntities();

                User user = db.Users.FirstOrDefault(i => i.Password == passwordHash && i.EmailAddress == viewModel.EmailAddress);

                if (user != null)
                    CookieService.SetCookie(Response, "UserID", 1, user.ID.ToString(CultureInfo.InvariantCulture));
                else
                {
                    ModelState.AddModelError("", "Invalid email or password!");
                    return View(viewModel);
                }
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult LogOff()
        {
            CookieService.DeleteCookie(Response, "UserID");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(LoginViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.EmailAddress))
            {
                ModelState.AddModelError("", "Email address is required!");
            }

            if (string.IsNullOrEmpty(viewModel.AccountName))
            {
                ModelState.AddModelError("", "Account Name is required!");
            }

            if (string.IsNullOrEmpty(viewModel.Password))
            {
                ModelState.AddModelError("", "Password is required!");
            }

            if (ModelState.IsValid)
            {
                var db = new MeatGrinderEntities();
                User existingUser = db.Users.FirstOrDefault(m => m.AccountName == viewModel.AccountName);

                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Account name already exists. Please try again.");
                    return View("Login", viewModel);
                }

                existingUser = db.Users.FirstOrDefault(m => m.EmailAddress == viewModel.EmailAddress);

                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email address already exists. Please try again");
                    return View("Login", viewModel);
                }

                var user = new User
                {
                    AccountName = viewModel.AccountName,
                    EmailAddress = viewModel.EmailAddress,
                    Password = GetHash(viewModel.Password),
                    DateCreated = DateTime.Now
                };

                db.Users.Add(user);
                db.SaveChanges();
                CookieService.SetCookie(Response, "UserID", 1, user.ID.ToString(CultureInfo.InvariantCulture));
                return RedirectToAction("Index");
            }

            return View("Login", viewModel);
        }

        [HttpGet]
        public ActionResult Landing()
        {
            return new FilePathResult("/Views/Home/landing.html", "text/html");
        }

        private string GetHash(string password)
        {
            MD5 md5Hash = MD5.Create();

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
    }
}
