
using MediatR;
using Smartplug.Core.Dtos;
using Smartplug.Domain.Enums;

namespace Smartplug.Application.Handlers.Schedule.Commands;
public class CreateScheduleCommand : IRequest<Response<string>>
{
    public Guid DeviceId { get; set; }
    public ScheduleType Type { get; set; }
    
    // Tek seferlik için:
    public DateTime? ScheduledTime { get; set; }
    public bool? DesiredStatus { get; set; }
    
    // Tekrarlayan için:
    public DayOfWeek? RecurringDay { get; set; }
    public TimeSpan? StartTimeOfDay { get; set; }
    public TimeSpan? EndTimeOfDay { get; set; }
}

