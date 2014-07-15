namespace MeatGrinder.DAL.Models
{
    public class TodoViewModel
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string TaskType { get; set; }
        public string GoalName { get; set; }
        public string ParentTaskName { get; set; }
    }
}