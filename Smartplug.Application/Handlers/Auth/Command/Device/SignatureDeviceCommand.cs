using MediatR;
using Smartplug.Application.Dtos.Auth.Device;
using Smartplug.Application.Services;
using Smartplug.Core.Dtos;
using Smartplug.Persistence;

namespace Smartplug.Application.Handlers.Auth.Command.Device;

public class SignatureDeviceCommand : IRequest<Response<string>>
{
    public SignatureDeviceRequest Data { get; set; }
}

public class SignatureDeviceCommandHandler(SmartplugDbContext smartplugDbContext,IUserAccessor userAccessor) 
    : IRequestHandler<SignatureDeviceCommand, Response<string>>
{
    public async Task<Response<string>> Handle(SignatureDeviceCommand request, CancellationToken cancellationToken)
    {
        var key = request.Data.DeviceKey;

        var userId = userAccessor.User.Claims.First(x => x.Type == "userId").Value;
        var signature = $"{key}-{userId}";

        var device = new Domain.Entities.Device
        {
            UserId = Guid.Parse(userId),
            SerialNumber = signature
        };
        
        await smartplugDbContext.Devices.AddAsync(device, cancellationToken);
        await smartplugDbContext.SaveChangesAsync(cancellationToken);
        
        return Response<string>.Success(signature, 200);
    }
}