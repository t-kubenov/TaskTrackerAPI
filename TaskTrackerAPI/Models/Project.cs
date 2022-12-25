using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text.Json.Serialization;

namespace TaskTrackerAPI.Models
{
    public enum ProjectStatus
    {
        NotStarted,
        Active,
        Completed
    }

    // separated project body from project ID to prevent users from messing with IDs during POSTs/PUTs
    public class ProjectBody
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime CompletionDate { get; set; } = DateTime.Today.AddMonths(1);

        [Column(TypeName = "varchar(10)")]
        [Range(0, 2)]
        public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;

        [Range(0, 10)]
        public int Priority { get; set; } = 0;
    }

    public class Project: ProjectBody
    {
        public int Id { get; set; }
    }
}
