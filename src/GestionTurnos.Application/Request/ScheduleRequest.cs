namespace GestionTurnos.Application.Request
{
    public class ScheduleRequest
    {
        public Guid BranchId { get; set; }
        public DayOfWeek Day { get; set; } 
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public bool? IsDeleted { get; set; } 
    }
}