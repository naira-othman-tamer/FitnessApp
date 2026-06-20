using AuthenticationService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Infrastructure;

public interface IAuthenticationDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken);
}

public sealed class AuthenticationDataSeeder(
    RoleManager<IdentityRole<Guid>> roleManager,
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration) : IAuthenticationDataSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        foreach (var roleName in configuration.GetSection("Seed:Roles").Get<string[]>() ?? [])
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }

        var email = configuration["Seed:Administrator:Email"];
        var password = configuration["Seed:Administrator:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email.Trim().ToLowerInvariant(),
                Email = email.Trim().ToLowerInvariant(),
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            var created = await userManager.CreateAsync(user, password);
            if (!created.Succeeded)
                throw new InvalidOperationException(string.Join(" ", created.Errors.Select(x => x.Description)));
        }

        if (await roleManager.RoleExistsAsync("Admin") && !await userManager.IsInRoleAsync(user, "Admin"))
            await userManager.AddToRoleAsync(user, "Admin");
    }
}
