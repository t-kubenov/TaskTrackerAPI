using System.ComponentModel.DataAnnotations;

namespace TaskTrackerAPI.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime CompletionDate { get; set; } = DateTime.Now.AddMonths(1);

        //public enum Status { NotStarted, Active, Completed } // MSSQL does not support enums :(
        public int Status { get; set; } = 0; // NotStarted = 0, Active = 1, Completed = 2
        public int Priority { get; set; } = 0;


    }
}
