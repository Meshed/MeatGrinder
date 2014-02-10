using System.Collections.Generic;
using System.Web.Mvc;
using MeatGrinder.Helpers;
using MeatGrinder.Models;
using MeatGrinder.Repositories;

namespace MeatGrinder.Controllers
{
    [CustomAuthorize]
    public class GoalController : Controller
    {
        private readonly GoalRepository _goalRepository;

        public GoalController()
        {
            _goalRepository = new GoalRepository();
        }

        public ActionResult GetAll()
        {
            var viewModel = new GoalViewModel
            {
                Goals = _goalRepository.GetAllForUser()
            };

            viewModel.Goals = UpdateChildTaskCounts(viewModel.Goals);

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public void Create(Goal goal)
        {
            if (ModelState.IsValid)
            {
                _goalRepository.Create(goal);
            }
        }

        public void Delete(Goal goal)
        {
            _goalRepository.Delete(goal);
        }

        private List<Goal> UpdateChildTaskCounts(List<Goal> goals)
        {
            foreach (var goal in goals)
            {
                goal.ChildTaskCount = _goalRepository.GetChildTaskCount(goal.ID);
                if (goal.ChildTaskCount > 0)
                {
                    goal.Description += " (" + goal.ChildTaskCount + ")";
                }
            }

            return goals;
        }
    }
}
