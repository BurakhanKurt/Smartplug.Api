namespace Smartplug.Domain.Entities
{
    public class EnergyUsageLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DeviceId { get; set; }
        public Device Device { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public double PowerUsageInWatts { get; set; }
        public double Voltage { get; set; }
        public double Current { get; set; }
    }
}
