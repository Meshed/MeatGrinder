using System.Collections.Generic;
using System.Linq;
using MeatGrinder.Models;
using MeatGrinder.Services;

namespace MeatGrinder.Repositories
{
    public class GoalRepository
    {
        private readonly MeatGrinderEntities _db = new MeatGrinderEntities();

        public Goal GetByID(int id)
        {
           return _db.Goals.FirstOrDefault(m => m.ID == id);
        }
        public int GetChildTaskCount(int goalID)
        {
            return _db.Tasks.Count(m => m.GoalID == goalID && m.ParentTaskID == null);
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
        public List<Goal> GetAllForUser()
        {
            int userID = CookieService.GetUserID();
            return _db.Goals.Where(m => m.UserID == userID).ToList();
        }
        public void Create(Goal goal)
        {
            goal.UserID = CookieService.GetUserID();
            goal.IsComplete = false;
            _db.Goals.Add(goal);
            _db.SaveChanges();
        }
        public void Delete(Goal goal)
        {
            var taskRepository = new TaskRepository();
            goal = _db.Goals.FirstOrDefault(m => m.ID == goal.ID);

            if (goal != null)
            {
                var childTasks = _db.Tasks.Where(m => m.GoalID == goal.ID);
                foreach (var childTask in childTasks)
                {
                    taskRepository.Delete(childTask);
                }
            }

            _db.Goals.Remove(goal);
            _db.SaveChanges();
        }
    }
}