using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Smartplug.Application.Services
{
    public class UserAccessor(IHttpContextAccessor accessor) : IUserAccessor
    {
        private readonly IHttpContextAccessor _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));

        public ClaimsPrincipal User => _accessor.HttpContext.User;

        public HttpContext HttpContext => _accessor.HttpContext;
    }
}
