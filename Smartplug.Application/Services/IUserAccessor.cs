using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Smartplug.Application.Services
{
    public interface IUserAccessor
    {
        ClaimsPrincipal User { get; }
        HttpContext HttpContext { get; }
    }
}
