using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smartplug.Application.Handlers.Schedule.Commands;
using Smartplug.Application.Handlers.Schedule.Queries;
using Smartplug.Core.Dtos;


namespace Smartplug.Api.Controllers
{


[Route("api/[controller]s")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public ScheduleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Tüm zamanlama ayarlarını listeleme (örneğin belirli bir cihaz için)
    [HttpGet("device/{deviceId}")]
    [Authorize]
    public async Task<IActionResult> GetSchedulesByDevice([FromRoute] Guid deviceId)
    {
        var response = await _mediator.Send(new GetSchedulesByDeviceQuery { DeviceId = deviceId });
        return Ok(response);
    }

    // Yeni zamanlama ayarı ekleme
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }
}
}
