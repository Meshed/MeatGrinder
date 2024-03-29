﻿using System.Collections.Generic;
using System.Linq;
using MeatGrinder.Models;
using MeatGrinder.Services;

namespace MeatGrinder.Repositories
{
    public class TaskRepository
    {
        private readonly MeatGrinderEntities _db = new MeatGrinderEntities();

        public Task GetByID(int id)
        {
            return _db.Tasks.FirstOrDefault(m => m.ID == id);
        }
        public List<Task> GetAllByGoalID(int goalId)
        {
            int userID = CookieService.GetUserID();
            return _db.Tasks.Where(m => m.GoalID == goalId &&
                                        m.ParentTaskID == null &&
                                        m.UserID == userID).ToList();
        }
        public List<Task> GetAllByTaskID(int taskId)
        {
            int userID = CookieService.GetUserID();
            return _db.Tasks.Where(m => m.ParentTaskID == taskId &&
                                        m.UserID == userID).ToList();
        }
        public int GetChildTaskCount(int taskID)
        {
            return _db.Tasks.Count(m => m.ParentTaskID == taskID);
        }
        public void Create(Task task)
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
                var goalRepository = new GoalRepository();

                goalRepository.SetGoalNotComplete(task.GoalID);
            }
        }
        public void SetTaskNotComplete(int taskId)
        {
            var task = _db.Tasks.FirstOrDefault(m => m.ID == taskId);

            if (task != null && task.IsComplete)
            {
                task.IsComplete = false;
                _db.SaveChanges();
            }
        }
        public void Delete(Task task)
        {
            task = _db.Tasks.FirstOrDefault(m => m.ID == task.ID);

            if (task != null) 
                DeleteTask(task);

            _db.SaveChanges();
        }

        private void DeleteTask(Task task)
        {
            var childTasks = _db.Tasks.Where(m => m.ParentTaskID == task.ID);

            foreach (var childTask in childTasks)
            {
                DeleteTask(childTask);
            }

            _db.Tasks.Remove(task);
        }
    }
}