using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MeatGrinder.Helpers;

using MeatGrinder.Repositories;
using MeatGrinder.Schema;

namespace MeatGrinder.Controllers
{
    using MeatGrinder.DAL.Models;

    [CustomAuthorize]
    public class TaskController : Controller
    {
        private readonly GoalRepository _goalRepository;
        private readonly TaskRepository _taskRepository;

        public TaskController()
        {
            _goalRepository = new GoalRepository();
            _taskRepository = new TaskRepository();
        }

        [HttpPost]
        public ActionResult GetTasksForGoal(Goal goal)
        {
            goal = _goalRepository.GetByID(goal.ID);

            if (goal != null)
            {
                var tasks = _taskRepository.GetAllByGoalID(goal.ID);
                UpdateChildTaskCounts(tasks);
                
                var viewModel = new TaskViewModel(CreateBreadCrumbs(goal),tasks);
                
                return Json(viewModel);
            }

            return Json(new TaskViewModel(new List<BreadCrumbModel>(),null));
        }

        [HttpPost]
        public ActionResult GetTasksForTask(int taskId)
        {
            var task = _taskRepository.GetByID(taskId);

            if (task != null)
            {
                var tasks = _taskRepository.GetAllByTaskID(task.ID);
                this.UpdateChildTaskCounts(tasks);
                
                var viewModel = new TaskViewModel(CreateBreadCrumbs(task),tasks);

                return Json(viewModel);
            }

            return Json(new TaskViewModel(new List<BreadCrumbModel>(),null));
        }

        private void UpdateChildTaskCounts(List<Task> tasks)
        {
            foreach (var task in tasks)
            {
                task.ChildTaskCount = _taskRepository.GetChildTaskCount(task.ID);
                if (task.ChildTaskCount > 0)
                {
                    task.Description += " (" + task.ChildTaskCount + ")";                    
                }
            }

        }

        public void Create(Task task)
        {
            if (ModelState.IsValid)
            {
                _taskRepository.Create(task);
            }
        }
        public void Delete(Task task)
        {
            if (task != null) 
                _taskRepository.Delete(task);
        }

        private List<BreadCrumbModel> CreateBreadCrumbs(Goal goal)
        {
            var breadCrumbs = new List<BreadCrumbModel>
            {
                CreateBreadCrumb(goal.Description, goal.ID, "goal")
            };

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
                Task newTask = _taskRepository.GetByID(task.ParentTaskID.Value);
                if (newTask != null) 
                    BuildBreadCrumbList(newTask, breadCrumbs);
            }
            else
            {
                Goal goal = _goalRepository.GetByID(task.GoalID);
                if (goal != null) 
                    breadCrumbs.Add(CreateBreadCrumb(goal.Description, goal.ID, "goal"));
            }

            breadCrumbs.Add(CreateBreadCrumb(task.Description, task.ID, "task"));
        }
        private BreadCrumbModel CreateBreadCrumb(string description, int id, string breadCrumbType)
        {
            var breadCrumb = new BreadCrumbModel(description,"#" + breadCrumbType + "/" + id);

            return breadCrumb;
        }
    }
}
