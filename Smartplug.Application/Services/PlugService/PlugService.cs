using System.Text;
using System.Text.Json;

namespace Smartplug.Application.Services.PlugService
{
    public class PlugService : IPlugService
    {
        private readonly HttpClient _httpClient;

        public PlugService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> SetRelayOnAsync(string deviceIp, bool powerState)
        {
            var url = $"http://{deviceIp}/relay/on";
            var content = new StringContent(JsonSerializer.Serialize(new { power = powerState }), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SetRelayOffAsync(string deviceIp, bool powerState)
        {
            var url = $"http://{deviceIp}/relay/off";
            var content = new StringContent(JsonSerializer.Serialize(new { power = powerState }), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
