using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web.Mvc;
using MeatGrinder.Helpers;

using MeatGrinder.Services;

namespace MeatGrinder.Controllers
{
    using MeatGrinder.DAL.Models;
    using MeatGrinder.Schema;

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
                    var task = _db.Tasks.FirstOrDefault(m => m.ID == viewModel.Id);

                    if (task != null)
                    {
                        task.IsComplete = true;
                        _db.SaveChanges();
                    }
                    break;
                case "Goal":
                    var goal = _db.Goals.FirstOrDefault(m => m.ID == viewModel.Id);

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
            int userId = CookieService.GetUserID();

            var goals = _db.Goals.Where(m => m.IsComplete == false && m.UserID == userId);
            foreach (var goal in goals)
            {
                int goalId = goal.ID;
                var tasks = _db.Tasks.Where(m => m.IsComplete == false &&
                                                 m.GoalID == goalId &&
                                                 m.ParentTaskID == null &&
                                                 m.UserID == userId);
                if (!tasks.Any())
                {
                    todoList.Add(new TodoViewModel(goal.ID, goal.Description, "Goal", null, null));
                }
                else
                {
                    GetTodoListTasks(tasks, userId, todoList);
                }
            }

            return todoList;
        }
        private void GetTodoListTasks(IEnumerable<Task> tasks, int userId, List<TodoViewModel> todoList)
        {
            foreach (var parentTask in tasks)
            {
                int parentTaskID = parentTask.ID;
                var childTasks = _db.Tasks.Where(m => m.IsComplete == false &&
                                                      m.ParentTaskID == parentTaskID &&
                                                      m.UserID == userId);

                if (!childTasks.Any())
                {
                    AddTaskWithNoTasksToList(todoList, parentTask, parentTaskID);
                }
                else
                {
                    GetTodoListTasks(childTasks, userId, todoList);
                }
            }
        }

        private void AddTaskWithNoTasksToList(List<TodoViewModel> todoList, Task parentTask, int parentTaskID)
        {
            
            var goal = _db.Goals.FirstOrDefault(m => m.ID == parentTask.GoalID);
            var tempTask = _db.Tasks.FirstOrDefault(m => m.ID == parentTask.ParentTaskID);
            var todo = new TodoViewModel(parentTaskID, parentTask.Description, "Task",goal!=null?goal.Description:null,tempTask!=null?tempTask.Description: null);

            todoList.Add(todo);
        }
       
    }
}
