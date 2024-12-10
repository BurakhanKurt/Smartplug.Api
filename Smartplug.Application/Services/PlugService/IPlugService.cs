namespace Smartplug.Application.Services.PlugService
{
    public interface IPlugService
    {
        Task<bool> SetRelayOnAsync(string deviceIp, bool powerState);
        Task<bool> SetRelayOffAsync(string deviceIp, bool powerState);
    }
}
