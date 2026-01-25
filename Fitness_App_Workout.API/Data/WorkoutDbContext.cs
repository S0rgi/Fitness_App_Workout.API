using Fitness_App_Workout.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
namespace Fitness_App_Workout.API.Data
{
    public class WorkoutDbContext : DbContext
    {
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercise { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<ChallengeExercise> ChallengeExercises { get; set; }
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

            modelBuilder.Entity<WorkoutExercise>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasMany(e => e.Sets)
                    .WithOne(s => s.WorkoutExercise)
                    .HasForeignKey(s => s.WorkoutExerciseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WorkoutSet>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Reps).IsRequired();
                entity.Property(s => s.Weight).IsRequired();
            });
            modelBuilder.Entity<Challenge>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Message)
                      .HasMaxLength(500);

                entity.HasMany(c => c.Exercises)
                      .WithOne() // Без навигационного свойства обратно к Challenge
                      .HasForeignKey(e => e.ChallengeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ChallengeExercise>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);
            });
        }
    }
}
