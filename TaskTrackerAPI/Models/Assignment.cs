using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTrackerAPI.Models
{
    public enum AssignmentStatus
    {
        ToDo, 
        InProgress, 
        Done
    }

    // same thing as with the projects
    public class AssignmentBody
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "varchar(10)")]
        [Range(0, 2)]
        public AssignmentStatus Status { get; set; } = AssignmentStatus.ToDo;

        public string Description { get; set; } = string.Empty;

        [Range(0, 10)]
        public int Priority { get; set; } = 0;

        [ForeignKey("ProjectForeignKey")]
        public int ParentProjectId { get; set; } = 0;
    }

    public class Assignment: AssignmentBody
    {
        public int Id { get; set; }
    }
}
