using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ProgressTrackingService.Domain.Entities;
namespace ProgressTrackingService.Infrastructure.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base (options) { }

        public DbSet<BodyMeasurements> BodyMeasurements { get; set; }
        public DbSet<ProgressGoals> ProgressGoals { get; set; }
        public DbSet<ProgressSnapshots> progressSnapshots{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
