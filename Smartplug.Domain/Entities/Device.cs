namespace Smartplug.Domain.Entities
{
    public class Device
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public bool IsOnline { get; set; } = false;
        public Guid UserId { get; set; }
        public Users User { get; set; }
        public ICollection<EnergyUsageLog> EnergyUsageLogs { get; set; } = new List<EnergyUsageLog>();
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
