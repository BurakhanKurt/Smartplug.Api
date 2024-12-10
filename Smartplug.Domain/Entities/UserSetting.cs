namespace Smartplug.Domain.Entities
{
    public class UserSetting
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Users User { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
    }
}
