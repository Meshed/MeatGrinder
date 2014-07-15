using System.Linq;
using System.Web.Mvc;
using MeatGrinder.Helpers;

using MeatGrinder.Services;

namespace MeatGrinder.Controllers
{
    using MeatGrinder.DAL.Models;
    using MeatGrinder.Schema;

    [CustomAuthorize]
    public class AdminController : Controller
    {
        readonly MeatGrinderEntities _db = new MeatGrinderEntities();

        public ActionResult Summary()
        {
            var viewModel = new AdminSummaryViewModel(_db.Users.Count());
            
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult IsUserAdmin()
        {
            var db = new MeatGrinderEntities();
            bool isUserAdmin = false;

            int userId = CookieService.GetUserID();
            User user = db.Users.FirstOrDefault(m => m.ID == userId);

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
