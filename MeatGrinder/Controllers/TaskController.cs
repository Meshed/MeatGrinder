using System.Linq;
using System.Web.Mvc;
using MeatGrinder.Helpers;
using MeatGrinder.Models;
using MeatGrinder.Services;

namespace MeatGrinder.Controllers
{
    [CustomAuthorize]
    public class TaskController : Controller
    {
        private readonly MeatGrinderEntities _db = new MeatGrinderEntities();

        [HttpPost]
        public ActionResult GetTasksForGoal(Goal goal)
        {
            int userID = CookieService.GetUserID();

            var viewModel = new TaskViewModel
            {
                Tasks = _db.Tasks.Where(m => m.GoalID == goal.ID && 
                    m.ParentTaskID == null &&
                    m.UserID == userID).ToList()
            };

            return Json(viewModel);
        }
        [HttpPost]
        public ActionResult GetTasksForTask(Task task)
        {
            var userID = CookieService.GetUserID();

            var viewModel = new TaskViewModel
            {
                Tasks = _db.Tasks.Where(m => m.ParentTaskID == task.ID &&
                    m.UserID == userID).ToList()
            };

            return Json(viewModel);
        }

        public void Create(Task task)
        {
            if (ModelState.IsValid)
            {
                task.UserID = CookieService.GetUserID();
                task.IsComplete = false;
                _db.Tasks.Add(task);
                _db.SaveChanges();
            }
        }
    }
}
