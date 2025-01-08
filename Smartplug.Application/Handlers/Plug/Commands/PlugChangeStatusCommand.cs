using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Smartplug.Application.Scoket;
using Smartplug.Core.Dtos;
using Smartplug.Persistence;

namespace Smartplug.Application.Handlers.Plug.Commands;

public class PlugChangeStatusCommand : IRequest<Response<NoContent>>
{
    public Guid DeviceId { get; set; }
    public bool Status { get; set; }
}

public class PlugChangeStatusCommandHandler(SmartplugDbContext dbContext,IHubContext<PlugHub> hubContext) 
    : IRequestHandler<PlugChangeStatusCommand, Response<NoContent>>
{
    public async Task<Response<NoContent>> Handle(PlugChangeStatusCommand request, CancellationToken cancellationToken)
    {
        var device = await dbContext
            .Devices
            .FirstOrDefaultAsync(x => x.Id == request.DeviceId,cancellationToken);
        
        if(device == null)
            return Response<NoContent>.Fail("Device not found", 404);

        if (device.IsWorking == request.Status)
            return Response<NoContent>.Fail("Device already in requested status", 400);
        
        if(!device.IsOnline)
            return Response<NoContent>.Fail("Device is offline", 400);
        
        PlugHub.ConnectedClients.TryGetValue(request.DeviceId, out var connectionId);
        if (connectionId != null)
        {
            await hubContext.Clients.Client(connectionId).SendAsync("PlugStatus", request.Status ? "on" : "off", cancellationToken);
        }
        
        return Response<NoContent>.Success(200);
    }
}