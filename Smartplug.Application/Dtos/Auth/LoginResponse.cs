using System.Text.Json.Serialization;

namespace Smartplug.Application.Dtos.Auth
{
    public class LoginResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Token { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RefreshToken { get; set; }
    }
}
