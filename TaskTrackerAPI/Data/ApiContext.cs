using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Models;

namespace TaskTrackerAPI.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Project>()
        //        .Property(u => u.Status)
        //        .HasConversion<string>()
        //        .HasMaxLength(10);
        //}

        public ApiContext(DbContextOptions<ApiContext> options): base(options) { }
    }
}
