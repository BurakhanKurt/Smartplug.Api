namespace Smartplug.Application.Handlers.Plug.Command
{
    public class SetPowerCommand
    {
        public string DeviceIp { get; set; }
        public bool PowerState { get; set; } // true: Aç, false: Kapat
    }
    public class SetPowerCommandHandler
    {
    }
}
