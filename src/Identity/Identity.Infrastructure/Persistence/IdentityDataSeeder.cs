using Identity.Domain.Constants;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Persistence;

public static class IdentityDataSeeder
{
    private const string DefaultAdminEmail = "admin@identity.local";
    private const string DefaultAdminPassword = "Admin@12345!";

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("IdentityDataSeeder");

        foreach (var roleName in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                if (result.Succeeded)
                {
                    logger.LogInformation("Seeded role {Role}", roleName);
                }
                else
                {
                    logger.LogWarning(
                        "Failed to seed role {Role}: {Errors}",
                        roleName,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        var admin = await userManager.FindByEmailAsync(DefaultAdminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = DefaultAdminEmail,
                UserName = DefaultAdminEmail,
                EmailConfirmed = true,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                LockoutEnabled = true
            };

            var createResult = await userManager.CreateAsync(admin, DefaultAdminPassword);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Admin);
                logger.LogInformation("Seeded default admin user {Email}", DefaultAdminEmail);
            }
            else
            {
                logger.LogWarning(
                    "Failed to seed admin user: {Errors}",
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }
        else if (!await userManager.IsInRoleAsync(admin, Roles.Admin))
        {
            await userManager.AddToRoleAsync(admin, Roles.Admin);
        }
    }
}
