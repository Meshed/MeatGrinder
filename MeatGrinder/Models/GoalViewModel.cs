using System.Collections.Generic;

namespace MeatGrinder.Models
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