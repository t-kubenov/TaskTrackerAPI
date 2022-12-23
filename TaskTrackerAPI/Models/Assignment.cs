using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("ProjectForeignKey")]
        public int ParentProjectId { get; set; }

    }
}
