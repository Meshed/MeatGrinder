using System.Collections.Generic;

namespace MeatGrinder.DAL.Models
{
    public class GoalViewModel
    {
        public List<Goal> Goals { get; set; }

        public GoalViewModel()
        {
            Goals = new List<Goal>();
        }
    }
}