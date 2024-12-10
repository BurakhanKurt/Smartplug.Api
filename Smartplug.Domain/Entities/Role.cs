using Microsoft.AspNetCore.Identity;

namespace Smartplug.Domain.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public bool IsDeleted { get; set; }
    }
}
