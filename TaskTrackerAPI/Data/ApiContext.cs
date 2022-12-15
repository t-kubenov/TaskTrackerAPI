using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Models;

namespace TaskTrackerAPI.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<TrackedTask> Tasks { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options): base(options) { }
    }
}
