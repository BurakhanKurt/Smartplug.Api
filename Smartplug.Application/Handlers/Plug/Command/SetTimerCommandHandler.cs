namespace Smartplug.Application.Handlers.Plug.Command
{
    public class SetTimerCommand
    {
        public string DeviceIp { get; set; }
        public bool PowerState { get; set; } // true: Aç, false: Kapat
        public DateTime ScheduledTime { get; set; } // UTC zaman formatı
    }
    internal class SetTimerCommandHandler
    {
    }
}
