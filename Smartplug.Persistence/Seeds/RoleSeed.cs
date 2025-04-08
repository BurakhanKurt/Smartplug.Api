using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Smartplug.Domain.Entities;
using Smartplug.Domain.Enums;
using Smurtplug.Persistence.Seeds.SeedRoles;
using System.Security.Claims;

namespace Smartplug.Persistence.Seeds
{
    public static class RoleSeed
    {
        public static async Task SeedRoles(IServiceProvider _serviceProvider)
        {
            using var scope = _serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            var roles = DefaultSeedRoles.SeedRoles as Dictionary<Roles, List<string>>;

            if (!roleManager.Roles.Any())
            {
                foreach (var role in roles)
                {
                    var roleResult = await roleManager.CreateAsync(new Role { Name = role.Key.ToString(), IsDeleted = false });

                    if (roleResult.Succeeded)
                    {
                        var systemRole = await roleManager.FindByNameAsync(role.Key.ToString());
                        var claims = role.Value.Select(x => new Claim(x, "")).ToList();
                        foreach (var claim in claims)
                        {
                            await roleManager.AddClaimAsync(systemRole, claim);
                        }
                    }

                }
            }//
            foreach (var role in roles)
            {
                if (await roleManager.RoleExistsAsync(role.Key.ToString()))
                    continue;

                var roleResult = await roleManager.CreateAsync(new Role { Name = role.Key.ToString(), IsDeleted = false });

                if (roleResult.Succeeded)
                {
                    var systemRole = await roleManager.FindByNameAsync(role.Key.ToString());
                    var claims = role.Value.Select(x => new Claim(x, "")).ToList();
                    foreach (var claim in claims)
                    {
                        await roleManager.AddClaimAsync(systemRole, claim);
                    }
                }
            }
        }

    }

}
