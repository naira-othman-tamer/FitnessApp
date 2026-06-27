using FCE.Domain.Aggregates;
using FCE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FCE.Infrastructure.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base (options) { }

        public DbSet<UserFitnessStats> UserFitnessStats { get; set; }
        public DbSet<CalculatedMetrics> CalculatedMetrics { get; set; }
        public DbSet<UserAssignedPlan> UserAssignedPlans { get; set; }
        public DbSet<TargetPlan> PlanRules { get; set; }
        public DbSet<UserPlanHistory> UserPlanHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
