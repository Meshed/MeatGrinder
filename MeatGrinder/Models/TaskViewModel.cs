using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeatGrinder.Models
{
    public class TaskViewModel
    {
        public List<Task> Tasks { get; set; }

        public TaskViewModel()
        {
            Tasks = new List<Task>();
        }
    }
}