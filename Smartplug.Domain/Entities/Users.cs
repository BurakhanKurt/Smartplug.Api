
using Microsoft.AspNetCore.Identity;
using Smartplug.Core.Entity;

namespace Smartplug.Domain.Entities
{
    public class Users : IdentityUser<Guid>, IEntityBase
    {
        public DateTime? CreatedAt { get; set; }//
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeleteDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsDeleted { get; set; }
        public string SecondPhoneNumber { get; set; }
    }

}
