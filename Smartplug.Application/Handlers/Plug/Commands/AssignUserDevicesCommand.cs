using MediatR;
using Microsoft.EntityFrameworkCore;
using Smartplug.Core.Dtos;
using Smartplug.Domain.Entities;
using Smartplug.Persistence;

namespace Smartplug.Application.Handlers.Plug.Commands;

public class AssignUserDevicesCommand : IRequest<Response<string>>
{
    public string UserId { get; set; }
    public string DevicesMac { get; set; }
    public string LocalIP { get; set; }
}

public class AssignUserDevicesCommandHandler(SmartplugDbContext dbContext)
    : IRequestHandler<AssignUserDevicesCommand, Response<string>>
{
    public async Task<Response<string>> Handle(AssignUserDevicesCommand request, CancellationToken cancellationToken)
    {
        var userExists = await dbContext
            .Users
            .AnyAsync(x => x.Id == Guid.Parse(request.UserId), cancellationToken);
        
        if(!userExists)
            return Response<string>.Fail("", 404);


        var device = new Device
        {
            Mac = request.DevicesMac,
            LocalIP = request.LocalIP,
            UserId = Guid.Parse(request.UserId),
            IsOnline = true,
            Name = "New Device"
        };
        
        await dbContext.Devices.AddAsync(device, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Response<string>.Success(device.Id.ToString(), 200);
    }
}