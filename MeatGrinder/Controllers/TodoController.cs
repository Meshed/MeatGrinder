using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web.Mvc;
using MeatGrinder.Helpers;
using MeatGrinder.Models;
using MeatGrinder.Services;

namespace MeatGrinder.Controllers
{
    [CustomAuthorize]
    public class TodoController : Controller
    {
        private readonly MeatGrinderEntities _db = new MeatGrinderEntities();

        public ActionResult GetList()
        {
            List<TodoViewModel> viewModel = GetToDoList();

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

        private List<TodoViewModel> GetToDoList()
        {
            var todoList = new List<TodoViewModel>();
            int userID = CookieService.GetUserID();

            var goals = _db.Goals.Where(m => m.IsComplete == false && m.UserID == userID);
            foreach (var goal in goals)
            {
                int goalID = goal.ID;
                var tasks = _db.Tasks.Where(m => m.IsComplete == false &&
                                                 m.GoalID == goalID &&
                                                 m.ParentTaskID == null &&
                                                 m.UserID == userID);
                if (!tasks.Any())
                {
                    AddGoalWithNoTasksToList(todoList, goal);
                }
                else
                {
                    GetTodoListTasks(tasks, userID, todoList);
                }
            }

            return todoList;
        }
        private void GetTodoListTasks(IEnumerable<Task> tasks, int userID, List<TodoViewModel> todoList)
        {
            foreach (var parentTask in tasks)
            {
                int parentTaskID = parentTask.ID;
                var childTasks = _db.Tasks.Where(m => m.IsComplete == false &&
                                                      m.ParentTaskID == parentTaskID &&
                                                      m.UserID == userID);
                if (!childTasks.Any())
                {
                    AddTaskWithNoTasksToList(todoList, parentTask, parentTaskID);
                }
                else
                {
                    GetTodoListTasks(childTasks, userID, todoList);
                }
            }
        }
        private static void AddTaskWithNoTasksToList(List<TodoViewModel> todoList, Task parentTask, int parentTaskID)
        {
            todoList.Add(new TodoViewModel
            {
                Description = parentTask.Description,
                ID = parentTaskID,
                TaskType = "Task"
            });
        }
        private static void AddGoalWithNoTasksToList(List<TodoViewModel> todoList, Goal goal)
        {
                todoList.Add(new TodoViewModel {Description = goal.Description, ID = goal.ID, TaskType = "Goal"});
        }
    }
}
