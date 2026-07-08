using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Infrastructure.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base (options) { }

        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<Domain.Entities.Workout> Workouts { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises{ get; set; }
        public DbSet<PlanDay> WorkoutPlanDays { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutSession> WorkoutSessions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
