using FCE.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace FCE.Configs.Extensions
{
    public static class ContextConfig
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Context>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("cs"))
                   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                   .LogTo(log => Debug.WriteLine(log), LogLevel.Information)
            );

            //services.AddScoped<IDbConnection>(_ =>
            //new SqlConnection(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
