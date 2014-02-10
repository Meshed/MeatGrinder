using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MeatGrinder.Helpers;
using MeatGrinder.Models;
using MeatGrinder.Repositories;

namespace MeatGrinder.Controllers
{
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

            var viewModel = new TaskViewModel();

            if (goal != null)
            {
                viewModel.Tasks = _taskRepository.GetAllByGoalID(goal.ID);
                viewModel.BreadCrumbs = CreateBreadCrumbs(goal);
            }

            viewModel.Tasks = UpdateChildTaskCounts(viewModel.Tasks);

            return Json(viewModel);
        }
        [HttpPost]
        public ActionResult GetTasksForTask(Task task)
        {
            task = _taskRepository.GetByID(task.ID);

            var viewModel = new TaskViewModel();
            if (task != null)
            {
                viewModel.Tasks = _taskRepository.GetAllByTaskID(task.ID);
                viewModel.BreadCrumbs = CreateBreadCrumbs(task);
            }

            viewModel.Tasks = UpdateChildTaskCounts(viewModel.Tasks);

            return Json(viewModel);
        }

        private List<Task> UpdateChildTaskCounts(List<Task> tasks)
        {
            foreach (var task in tasks)
            {
                task.ChildTaskCount = _taskRepository.GetChildTaskCount(task.ID);
                if (task.ChildTaskCount > 0)
                {
                    task.Description += " (" + task.ChildTaskCount + ")";                    
                }
            }

            return tasks;
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
            var breadCrumb = new BreadCrumbModel
            {
                DisplayName = description, 
                Url = "#" + breadCrumbType + "/" + id
            };

            return breadCrumb;
        }
    }
}
