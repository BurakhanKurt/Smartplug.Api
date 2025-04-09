

using MediatR;
using Microsoft.EntityFrameworkCore;
using Smartplug.Application.Dtos.Schedule;
using Smartplug.Core.Dtos;
using Smartplug.Persistence;

namespace Smartplug.Application.Handlers.Schedule.Queries;


public class GetSchedulesByDeviceQueryHandler : IRequestHandler<GetSchedulesByDeviceQuery, Response<List<ScheduleDto>>>
{
    private readonly SmartplugDbContext _dbContext;

    public GetSchedulesByDeviceQueryHandler(SmartplugDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Response<List<ScheduleDto>>> Handle(GetSchedulesByDeviceQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _dbContext.Schedules
            .Where(s => s.DeviceId == request.DeviceId)
            .Select(s => new ScheduleDto
            {
                Id = s.Id,
                Type = s.Type,
                ScheduledTime = s.ScheduledTime,
                DesiredStatus = s.DesiredStatus,
                Executed = s.Executed,
                RecurringDay = s.RecurringDay,
                StartTimeOfDay = s.StartTimeOfDay,
                EndTimeOfDay = s.EndTimeOfDay,
                IsEnabled = s.IsEnabled
            })
            .ToListAsync(cancellationToken);
        
        return Response<List<ScheduleDto>>.Success(schedules, 200);
    }
}