namespace GestionTurnos.Application.Response
{
    public class ScheduleResponse
    {
        public Guid BranchId { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}