using System.Data.Objects;
using System.Linq;
using System.Web.Mvc;
using MeatGrinder.Helpers;
using MeatGrinder.Models;
using MeatGrinder.Services;

namespace MeatGrinder.Controllers
{
    public class TodoController : Controller
    {
        private readonly MeatGrinderEntities _db = new MeatGrinderEntities();

        [CustomAuthorize]
        public ActionResult GetList()
        {
            int userID = CookieService.GetUserID();

            ObjectResult<TodoList_GetAllForUser_Result> results = _db.TodoList_GetAllForUser(userID);
            var viewModel = results.Select(result => new TodoViewModel
            {
                ID = result.ID, Description = result.Description, TaskType = result.TaskType
            }).ToList();

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void Complete(TodoViewModel viewModel)
        {
            switch (viewModel.TaskType)
            {
                case "Task":
                    var task = _db.Tasks.FirstOrDefault(m => m.ID == viewModel.ID);

                    if (task != null)
                    {
                        task.IsComplete = true;
                        _db.SaveChanges();
                    }
                    break;
                case "Goal":
                    var goal = _db.Goals.FirstOrDefault(m => m.ID == viewModel.ID);

                    if (goal != null)
                    {
                        goal.IsComplete = true;
                        _db.SaveChanges();
                    }
                    break;
            }
        }
    }
}
