using Microsoft.AspNetCore.Http;

namespace Smartplug.Application.Services.PlugService
{
    public interface IPlugService
    {
        Task SendMessageAsync(string message);
        Task HandleAsync(HttpContext context);
        //Task<bool> SetRelayOffAsync(string deviceIp, bool powerState);
    }
}
