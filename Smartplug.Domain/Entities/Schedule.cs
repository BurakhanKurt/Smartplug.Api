namespace Smartplug.Domain.Entities
{
    public class Schedule
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DeviceId { get; set; }
        public Device Device { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string Name { get; set; }
        ///

    }
}
