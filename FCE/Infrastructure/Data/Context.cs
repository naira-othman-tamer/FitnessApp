using FCE.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FCE.Infrastructure.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base (options) { }

        public DbSet<UserFitnessStats> UserFitnessStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
