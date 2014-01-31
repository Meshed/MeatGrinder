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
    }
}
