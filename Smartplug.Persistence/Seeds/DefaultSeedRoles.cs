using Smartplug.Domain.Enums;

namespace Smurtplug.Persistence.Seeds.SeedRoles
{
    public static class DefaultSeedRoles
    {
        public static Dictionary<Roles, List<string>> SeedRoles = new Dictionary<Roles, List<string>>()
        {

            {
                Roles.ADMIN, new List<string>()
                {

                }
            }
           
        };
    }
}