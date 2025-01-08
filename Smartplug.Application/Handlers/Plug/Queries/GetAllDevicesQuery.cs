using MediatR;
using Microsoft.EntityFrameworkCore;
using Smartplug.Application.Dtos.Plug.Response;
using Smartplug.Application.Services;
using Smartplug.Core.Dtos;
using Smartplug.Persistence;

namespace Smartplug.Application.Handlers.Plug.Queries;

public class GetAllDevicesQuery : IRequest<Response<List<GetAllDevicesResponse>>>;

public class GetAllDevicesQueryHandler(SmartplugDbContext dbContext,IUserAccessor userAccessor) 
    : IRequestHandler<GetAllDevicesQuery, Response<List<GetAllDevicesResponse>>>
{
    public async Task<Response<List<GetAllDevicesResponse>>> Handle(GetAllDevicesQuery request, CancellationToken cancellationToken)
    {
        var userId = userAccessor.User.Claims.FirstOrDefault(x => x.Type.Equals("userid"))?.Value;
        var devices = await dbContext.Devices
            .Where(x => x.UserId == Guid.Parse(userId??string.Empty))
            .Select(x => new GetAllDevicesResponse
            {
                Id = x.Id,
                Name = x.Name,
                SerialNumber = x.SerialNumber,
                IsOnline = x.IsOnline,
                isWorking = x.IsWorking,
                LocalIP = x.LocalIP,
                Mac= x.Mac,
            })
            .ToListAsync(cancellationToken);
        
        return Response<List<GetAllDevicesResponse>>.Success(devices,200);
    }
}