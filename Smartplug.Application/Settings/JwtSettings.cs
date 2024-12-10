namespace Smartplug.Application.Settings
{
    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }
        public int Ttl { get; set; }
        public int RefreshTtl { get; set; }
    }
}
