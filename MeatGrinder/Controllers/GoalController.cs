using System.Linq;
using System.Web.Mvc;
using MeatGrinder.Helpers;
using MeatGrinder.Models;
using MeatGrinder.Services;

namespace MeatGrinder.Controllers
{
    public class GoalController : Controller
    {
        private readonly MeatGrinderEntities _db = new MeatGrinderEntities();

        [CustomAuthorize]
        public ActionResult GetAll()
        {
            int userID = CookieService.GetUserID();
            var viewModel = new GoalViewModel
            {
                Goals = _db.Goals.Where(m => m.UserID == userID).ToList()
            };

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize]
        public void Create(Goal goal)
        {
            if (ModelState.IsValid)
            {
                goal.UserID = CookieService.GetUserID();
                _db.Goals.Add(goal);
                _db.SaveChanges();
            }
        }
    }
}
