using System.ComponentModel.DataAnnotations;

namespace TaskTrackerAPI.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        //public enum Status { ToDo, InProgress, Done}
        public int Status { get; set; } = 0; // ToDo = 0, InProgress = 1, Done = 2
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; } = 0;
        public Project? ParentProject { get; set; }

    }
}
