using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smartplug.Application.Handlers.Auth.Command.Device;
using Smartplug.Application.Handlers.Plug.Commands;
using Smartplug.Application.Handlers.Plug.Queries;
using Smartplug.Core.ControllerBases;

namespace Smartplug.Api.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class PlugController(IMediator mediator) : CustomControllerBase
    {
        [Authorize]
        [HttpGet("all-devices")]
        public async Task<IActionResult> GetAllDevices()
        {
            var response = await mediator.Send(new GetAllDevicesQuery());
            return CreateActionResultInstance(response);
        }
        
        [Authorize]
        [HttpPut("devices")]
        public async Task<IActionResult> PutDevice([FromBody] UpdateDeviceCommand command)
        {
            var response = await mediator.Send(command);
            return CreateActionResultInstance(response);
        }

        [HttpPost("assign-user-devices")]
        public async Task<IActionResult> AssignUserDevices([FromBody] AssignUserDevicesCommand command)
        {
            var response = await mediator.Send(command);
            return CreateActionResultInstance(response);
        }

        [Authorize]
        [HttpGet("plug-status/{deviceId}")]
        public async Task<IActionResult> PlugChangeStatus(
            [FromRoute] Guid deviceId,
            [FromQuery] bool status)
        {
            var response = await mediator.Send(new PlugChangeStatusCommand()
            {
                DeviceId = deviceId,
                Status = status
            });
            return CreateActionResultInstance(response);
        }

        [Authorize]
        [HttpGet("healty-checkOne")]
        public async Task<IActionResult> PlugChangeStatus()
        {
            return Ok();
        }

        [HttpDelete("device")]
        public async Task<IActionResult> AssignUserDevices([FromRoute] Guid deviceId)
        {
            var response = await mediator.Send(new DeleteDeviceCommand { DeviceId = deviceId });
            return CreateActionResultInstance(response);
        }
    }
}