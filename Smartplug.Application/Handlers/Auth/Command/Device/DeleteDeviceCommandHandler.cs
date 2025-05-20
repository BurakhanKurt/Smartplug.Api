using MediatR;
using Microsoft.EntityFrameworkCore;
using Smartplug.Core.Dtos;
using Smartplug.Persistence;
using Smartplug.Application.Services;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Smartplug.Application.Scoket;

namespace Smartplug.Application.Handlers.Auth.Command.Device;

public class DeleteDeviceCommand : IRequest<Response<string>>
{
    public Guid DeviceId { get; set; }
}

public class DeleteDeviceCommandHandler(
    SmartplugDbContext dbContext,
    IUserAccessor userAccessor,
    IHubContext<PlugHub> hubContext
) : IRequestHandler<DeleteDeviceCommand, Response<string>>
{
    public async Task<Response<string>> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {

        var device = await dbContext.Devices
            .Include(x => x.EnergyUsageLogs)
            .Include(x => x.Schedules)
            .FirstOrDefaultAsync(x => x.Id == request.DeviceId, cancellationToken);

        if (device == null)
            return Response<string>.Fail("error.device.notfound", 404);

        PlugHub.ConnectedClients.TryGetValue(device.Id, out var connectionId);
        if (connectionId != null)
        {
            await hubContext.Clients.Client(connectionId).SendAsync("PlugStatus", "forceOffline", cancellationToken);
            PlugHub.ConnectedClients.TryRemove(device.Id, out _);
        }

        foreach (var schedule in device.Schedules)
        {
            if (!string.IsNullOrEmpty(schedule.HangfireJobId))
                BackgroundJob.Delete(schedule.HangfireJobId);

            if (!string.IsNullOrEmpty(schedule.HangfireJobIdOn))
                RecurringJob.RemoveIfExists(schedule.HangfireJobIdOn);

            if (!string.IsNullOrEmpty(schedule.HangfireJobIdOff))
                RecurringJob.RemoveIfExists(schedule.HangfireJobIdOff);
        }

        // 4. Veritabanından ilişkili tüm kayıtları sil
        dbContext.EnergyUsageLogs.RemoveRange(device.EnergyUsageLogs);
        dbContext.Schedules.RemoveRange(device.Schedules);
        dbContext.Devices.Remove(device);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Response<string>.Success("Device deleted successfully", 200);
    }
}
