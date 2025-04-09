using MediatR;
using Smartplug.Application.Dtos.Schedule;
using Smartplug.Core.Dtos;
namespace Smartplug.Application.Handlers.Schedule.Queries;
public class GetSchedulesByDeviceQuery : IRequest<Response<List<ScheduleDto>>>
{
    public Guid DeviceId { get; set; }
}