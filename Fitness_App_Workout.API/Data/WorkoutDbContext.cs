using Fitness_App_Workout.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
namespace Fitness_App_Workout.API.Data
{
    public class WorkoutDbContext : DbContext
    {
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public WorkoutDbContext(DbContextOptions<WorkoutDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Workout>(entity =>
            {
                entity.HasKey(w => w.Id);
                entity.Property(w => w.Title).IsRequired().HasMaxLength(100);
                entity.Property(w => w.Date).IsRequired();
                entity.Property(w => w.UserId).IsRequired();
                entity.HasMany(w => w.Exercises)
                      .WithOne()
                      .HasForeignKey(e => e.WorkoutId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Exercise>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Sets).IsRequired();
                entity.Property(e => e.Reps).IsRequired();
                entity.Property(e => e.Weight).IsRequired();
            });
        }
    }
}
