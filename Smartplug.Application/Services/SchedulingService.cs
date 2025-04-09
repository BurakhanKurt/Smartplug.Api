using MediatR;
using Smartplug.Application.Handlers.Plug.Commands;

public interface ISchedulingService
{
    Task ChangeDeviceStatusAsync(Guid deviceId, bool status);
}

public class SchedulingService : ISchedulingService
{
    private readonly IMediator _mediator;

    public SchedulingService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task ChangeDeviceStatusAsync(Guid deviceId, bool status)
    {
        // Burada mevcut komutunuzu yeniden kullanarak cihaz durumunu değiştirebilirsiniz.
        await _mediator.Send(new PlugChangeStatusCommand
        {
            DeviceId = deviceId,
            Status = status
        });
    }
}
