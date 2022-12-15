namespace TaskTrackerAPI.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public enum Status { NotStarted, Active, Completed }
        public int Priority { get; set; }


    }
}
