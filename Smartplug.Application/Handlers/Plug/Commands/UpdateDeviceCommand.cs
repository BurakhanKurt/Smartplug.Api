using MediatR;
using Microsoft.EntityFrameworkCore;
using Smartplug.Core.Dtos;
using Smartplug.Persistence;

namespace Smartplug.Application.Handlers.Plug.Commands;

public class UpdateDeviceCommand : IRequest<Response<NoContent>>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class UpdateDeviceCommandHandler(SmartplugDbContext dbContext)
    : IRequestHandler<UpdateDeviceCommand, Response<NoContent>>
{
    public async Task<Response<NoContent>> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
    {
        await dbContext.Devices.Where(x => x.Id == request.Id)
            .ExecuteUpdateAsync(s => 
                s.SetProperty(p => p.Name, request.Name), cancellationToken);

        return Response<NoContent>.Success(204);
    }
}