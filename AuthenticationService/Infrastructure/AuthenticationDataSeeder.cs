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

        await SeedUserAsync("Seed:Administrator", "Admin");
        await SeedUserAsync("Seed:User", "User");
    }

    private async Task SeedUserAsync(string configurationSection, string roleName)
    {
        var email = configuration[$"{configurationSection}:Email"];
        var password = configuration[$"{configurationSection}:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await userManager.FindByEmailAsync(normalizedEmail);
        if (user is null)
        {
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = normalizedEmail,
                Email = normalizedEmail,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            var created = await userManager.CreateAsync(user, password);
            if (!created.Succeeded)
                throw new InvalidOperationException(string.Join(" ", created.Errors.Select(x => x.Description)));
        }

        if (!await roleManager.RoleExistsAsync(roleName) || await userManager.IsInRoleAsync(user, roleName))
            return;

        var roleResult = await userManager.AddToRoleAsync(user, roleName);
        if (!roleResult.Succeeded)
            throw new InvalidOperationException(string.Join(" ", roleResult.Errors.Select(x => x.Description)));
    }
}
