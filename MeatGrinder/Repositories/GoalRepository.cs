using System.Linq;
using MeatGrinder.Models;

namespace MeatGrinder.Repositories
{
    public class GoalRepository
    {
        private readonly MeatGrinderEntities _db = new MeatGrinderEntities();

        public Goal GetByID(int id)
        {
           return _db.Goals.FirstOrDefault(m => m.ID == id);
        }
        public void SetGoalNotComplete(int goalId)
        {
            var goal = _db.Goals.FirstOrDefault(m => m.ID == goalId);

            if (goal != null && goal.IsComplete)
            {
                goal.IsComplete = false;
                _db.SaveChanges();
            }
        }
    }
}