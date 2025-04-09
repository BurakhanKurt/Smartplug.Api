using MediatR;
using Smartplug.Core.Dtos;
using Smartplug.Domain.Enums;
using Smartplug.Persistence;
using Hangfire;

namespace Smartplug.Application.Handlers.Schedule.Commands;

public class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, Response<string>>
{
    private readonly SmartplugDbContext _dbContext;
    private readonly ISchedulingService _schedulingService;

    public CreateScheduleCommandHandler(SmartplugDbContext dbContext, ISchedulingService schedulingService)
    {
        _dbContext = dbContext;
        _schedulingService = schedulingService;
    }

    public async Task<Response<string>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = new Domain.Entities.Schedule
        {
            DeviceId = request.DeviceId,
            Type = request.Type,
            IsEnabled = true
        };

        if (request.Type == ScheduleType.OneTime)
        {
            // Gelen scheduledTime değerinin Kind'ı Unspecified ise UTC olarak işaretliyoruz.
            var scheduledUtc = DateTime.SpecifyKind(request.ScheduledTime.Value, DateTimeKind.Utc);
            schedule.ScheduledTime = scheduledUtc;
            schedule.DesiredStatus = request.DesiredStatus;
            schedule.Executed = false;

            // Hesaplamada UTC kullanıyoruz
            var delay = scheduledUtc - DateTime.UtcNow;
            // Hangfire job planlaması (tek seferlik)
            schedule.HangfireJobId = BackgroundJob.Schedule<ISchedulingService>(
                service => service.ChangeDeviceStatusAsync(request.DeviceId, request.DesiredStatus.Value),
                delay);
        }
        else if (request.Type == ScheduleType.Recurring)
        {
            // Recurring işlemler için ilgili saat bilgileri ve gün ataması
            schedule.RecurringDay = request.RecurringDay;
            schedule.StartTimeOfDay = request.StartTimeOfDay;
            schedule.EndTimeOfDay = request.EndTimeOfDay;

            // Cihazın açılması için:
            var cronExpressionOn = Cron.Weekly(
                request.RecurringDay.Value,
                request.StartTimeOfDay.Value.Hours,
                request.StartTimeOfDay.Value.Minutes);
            schedule.HangfireJobIdOn = $"DeviceOn_{schedule.DeviceId}_{Guid.NewGuid()}";
            RecurringJob.AddOrUpdate<ISchedulingService>(
                schedule.HangfireJobIdOn,
                service => service.ChangeDeviceStatusAsync(request.DeviceId, true),
                cronExpressionOn);

            // Cihazın kapatılması için:
            var cronExpressionOff = Cron.Weekly(
                request.RecurringDay.Value,
                request.EndTimeOfDay.Value.Hours,
                request.EndTimeOfDay.Value.Minutes);
            schedule.HangfireJobIdOff = $"DeviceOff_{schedule.DeviceId}_{Guid.NewGuid()}";
            RecurringJob.AddOrUpdate<ISchedulingService>(
                schedule.HangfireJobIdOff,
                service => service.ChangeDeviceStatusAsync(request.DeviceId, false),
                cronExpressionOff);
        }

        await _dbContext.Schedules.AddAsync(schedule, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Response<string>.Success(schedule.Id.ToString(), 200);
    }
}
