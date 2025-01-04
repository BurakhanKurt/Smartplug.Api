using MediatR;
using Microsoft.Extensions.Logging;
using Smartplug.Application.Services.PlugService;
using Smartplug.Core.Dtos;
using System.Security.Cryptography.X509Certificates;

namespace Smartplug.Application.Handlers.Plug.Command
{
    public record SetRelayOnCommand : IRequest<Response<string>>
    {
    }


    public class SetRelayOnCommandHandler : IRequestHandler<SetRelayOnCommand, Response<string>>
    {
        private readonly IPlugService _plugService; // Assuming you have IPlugService injected
        private readonly ILogger<SetRelayOnCommandHandler> _logger;

        public SetRelayOnCommandHandler(IPlugService plugService, ILogger<SetRelayOnCommandHandler> logger)
        {
            _plugService = plugService;
            _logger = logger;
        }

        public async Task<Response<string>> Handle(SetRelayOnCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Send the "set_relay_on" message via WebSocket
                await _plugService.SendMessageAsync("set_relay_on");
                _logger.LogInformation("Sent set_relay_on message");

                // You can return a success response
                return Response<string>.Success("Relay is now ON", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending set_relay_on message");
                return Response<string>.Fail("Failed to turn on relay", 500);
            }
        }
    }


}
