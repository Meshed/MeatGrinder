using System.Collections.Generic;
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
            goal = _db.Goals.FirstOrDefault(m => m.ID == goal.ID);

            var viewModel = new TaskViewModel();

            if (goal != null)
            {
                viewModel.Tasks = _db.Tasks.Where(m => m.GoalID == goal.ID &&
                                                       m.ParentTaskID == null &&
                                                       m.UserID == userID).ToList();
                viewModel.BreadCrumbs = CreateBreadCrumbs(goal);
            }

            return Json(viewModel);
        }
        [HttpPost]
        public ActionResult GetTasksForTask(Task task)
        {
            var userID = CookieService.GetUserID();
            task = _db.Tasks.FirstOrDefault(m => m.ID == task.ID);

            var viewModel = new TaskViewModel();
            if (task != null)
            {
                viewModel.Tasks = _db.Tasks.Where(m => m.ParentTaskID == task.ID &&
                                                       m.UserID == userID).ToList();
                viewModel.BreadCrumbs = CreateBreadCrumbs(task);
            }

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

                if (task.ParentTaskID != null)
                {
                    SetTaskNotComplete(task.ParentTaskID.Value);
                }
                else
                {
                    SetGoalNotComplete(task.GoalID);
                }
            }
        }

        private void SetTaskNotComplete(int taskId)
        {
            var task = _db.Tasks.FirstOrDefault(m => m.ID == taskId);

            if (task != null && task.IsComplete)
            {
                task.IsComplete = false;
                _db.SaveChanges();
            }
        }
        private void SetGoalNotComplete(int goalId)
        {
            var goal = _db.Goals.FirstOrDefault(m => m.ID == goalId);

            if (goal != null && goal.IsComplete)
            {
                goal.IsComplete = false;
                _db.SaveChanges();
            }
        }
        private List<BreadCrumbModel> CreateBreadCrumbs(Goal goal)
        {
            var breadCrumbs = new List<BreadCrumbModel>();

            breadCrumbs.Add(CreateBreadCrumb(goal.Description, goal.ID, "goal"));

            return breadCrumbs;
        }
        private List<BreadCrumbModel> CreateBreadCrumbs(Task task)
        {
            var breadcrumbs = new List<BreadCrumbModel>();

            BuildBreadCrumbList(task, breadcrumbs);

            return breadcrumbs;
        }
        private void BuildBreadCrumbList(Task task, List<BreadCrumbModel> breadCrumbs)
        {
            if (task.ParentTaskID != null)
            {
                Task newTask = _db.Tasks.FirstOrDefault(m => m.ID == task.ParentTaskID);
                if (newTask != null) 
                    BuildBreadCrumbList(newTask, breadCrumbs);
            }
            else
            {
                Goal goal = _db.Goals.FirstOrDefault(m => m.ID == task.GoalID);
                if (goal != null) 
                    breadCrumbs.Add(CreateBreadCrumb(goal.Description, goal.ID, "goal"));
            }

            breadCrumbs.Add(CreateBreadCrumb(task.Description, task.ID, "task"));
        }
        private BreadCrumbModel CreateBreadCrumb(string description, int id, string breadCrumbType)
        {
            var breadCrumb = new BreadCrumbModel();
            breadCrumb.DisplayName = description;
            breadCrumb.Url = "#" + breadCrumbType + "/" + id;

            return breadCrumb;
        }
    }
}
