using Microsoft.EntityFrameworkCore;
using TaskMicroService.Entities;

namespace TaskMicroService.DataAccess
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
        {
        }

        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<TaskStep> TaskSteps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskEntity>()
                .HasMany(t => t.Steps) // En Task har mange Steps
                .WithOne(s => s.Task)   // Et Step hører til én Task
                .HasForeignKey(s => s.TaskId)  // Fremmednøgle på TaskId
                .OnDelete(DeleteBehavior.Cascade); // Sletter Steps, hvis Task slettes
            
            modelBuilder.Entity<TaskStep>()
                .HasOne(ts => ts.Task)
                .WithMany(t => t.Steps)
                .HasForeignKey(ts => ts.TaskId)
                .OnDelete(DeleteBehavior.Cascade); // Ensure TaskSteps are deleted with their parent Task


            base.OnModelCreating(modelBuilder);
        }

    }
}