namespace Smartplug.Application.Dtos.Plug.Response;

public class GetAllDevicesResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string SerialNumber { get; set; }
    public string LocalIP { get; set; }
    public string Mac { get; set; }
    public bool IsOnline { get; set; } = false;
}