using System.Collections.Generic;

namespace MeatGrinder.Models
{
    public class TaskViewModel
    {
        public List<Task> Tasks { get; set; }
        public List<BreadCrumbModel> BreadCrumbs { get; set; }

        public TaskViewModel()
        {
            Tasks = new List<Task>();
            BreadCrumbs = new List<BreadCrumbModel>();
        }
    }
}