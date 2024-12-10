using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Smartplug.Domain.Entities;
using Smartplug.Domain.Enums;

namespace Smartplug.Persistence.Seeds
{
    public static class DefaultSeedUsers
    {
        public static async Task SeedDefaultUser(IServiceProvider _serviceProvider)
        {

            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();

            #region SeedAdminUser
            var isExistAdminUser = (await userManager.GetUsersInRoleAsync(nameof(Roles.ADMIN))).Any();
            if (!isExistAdminUser)
            {
                Users user = new Users()
                {
                    Email = "admin@smurtplug.com",
                    UserName = "adminuser",
                    Name = "ADMIN",
                    Surname ="ADMIN",
                    SecondPhoneNumber = "",
                    LastLoginDate = DateTime.UtcNow,
                    PhoneNumberConfirmed = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "P@ssw0rd");
                await userManager.AddToRoleAsync(user, nameof(Roles.ADMIN));
            }
            #endregion
           
        }
        private static Roles ParsToRoles(string name)
        {
            return Enum.Parse<Roles>(name);
        }
    }
}


