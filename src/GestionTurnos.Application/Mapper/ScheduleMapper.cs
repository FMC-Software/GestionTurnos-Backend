using GestionTurnos.Domain.Entities; // Ajusta según tu namespace de dominios
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;

public static class ScheduleMapper
{
    public static Schedule ToEntitySchedule(this ScheduleRequest request)
    {
        return new Schedule
        {
            Id = Guid.NewGuid(), 
            BranchId = request.BranchId,
            DayOfWeek = request.Day,
            StartTime = request.StartTime ?? default(TimeSpan),
            EndTime = request.EndTime ?? default(TimeSpan),
            IsDeleted = false 
        };
    }

    public static ScheduleResponse ToResponseSchedule(this Schedule entity)
    {
        return new ScheduleResponse
        {
            BranchId = entity.BranchId,
            Day = entity.DayOfWeek,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime
        };
    }
}