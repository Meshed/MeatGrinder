using System.Linq;
using System.Web.Mvc;
using MeatGrinder.Helpers;
using MeatGrinder.Models;
using MeatGrinder.Services;

namespace MeatGrinder.Controllers
{
    [CustomAuthorize]
    public class AdminController : Controller
    {
        readonly MeatGrinderEntities _db = new MeatGrinderEntities();

        public ActionResult Summary()
        {
            var viewModel = new AdminSummaryViewModel
            {
                TotalUserCount = _db.Users.Count()
            };

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult IsUserAdmin()
        {
            var db = new MeatGrinderEntities();
            bool isUserAdmin = false;

            int userID = CookieService.GetUserID();
            User user = db.Users.FirstOrDefault(m => m.ID == userID);

            if (user != null)
            {
                if (user.EmailAddress == "marksbrown@gmail.com")
                {
                    isUserAdmin = true;
                }
            }

            return Json(isUserAdmin, JsonRequestBehavior.AllowGet);
        }
    }
}
