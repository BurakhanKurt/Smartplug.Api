using Smartplug.Domain.Enums;
namespace Smartplug.Application.Dtos.Schedule;
public class ScheduleDto
{
    public Guid Id { get; set; }
    public ScheduleType Type { get; set; }
    public DateTime? ScheduledTime { get; set; }
    public bool? DesiredStatus { get; set; }
    public bool? Executed { get; set; }
    public DayOfWeek? RecurringDay { get; set; }
    public TimeSpan? StartTimeOfDay { get; set; }
    public TimeSpan? EndTimeOfDay { get; set; }
    public bool IsEnabled { get; set; }
}