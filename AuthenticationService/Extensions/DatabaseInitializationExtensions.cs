using AuthenticationService.Data;
using AuthenticationService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Extensions;

public static class DatabaseInitializationExtensions
{
    public static async Task MigrateAndSeedAuthenticationDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("AuthenticationDatabaseInitialization");

        try
        {
            var context = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
            await context.Database.MigrateAsync();
            await scope.ServiceProvider.GetRequiredService<IAuthenticationDataSeeder>()
                .SeedAsync(CancellationToken.None);
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Authentication database migration or seeding failed.");
            throw;
        }
    }
}
