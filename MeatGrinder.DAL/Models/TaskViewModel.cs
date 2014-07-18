using MeatGrinder.Schema;
using System.Collections.Generic;

namespace MeatGrinder.DAL.Models
{
    public class TaskViewModel : BreadCrumbWrapper
    {
        public IList<Task> Tasks { get; private set; }
        
        public TaskViewModel(List<BreadCrumbModel> breadCrumbs,IList<Task> tasks):base(breadCrumbs)
        {
            Tasks = tasks??new List<Task>() ;
        }
    }
}