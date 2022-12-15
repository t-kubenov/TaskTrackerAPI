namespace TaskTrackerAPI.Models
{
    public class TrackedTask
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public enum Status { ToDo, InProgress, Done}
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public Project? ParentProject { get; set; }

    }
}
