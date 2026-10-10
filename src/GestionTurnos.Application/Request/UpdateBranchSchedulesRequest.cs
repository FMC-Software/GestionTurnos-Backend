namespace GestionTurnos.Application.Request
{
    public class UpdateBranchSchedulesRequest
    {
        public List<ScheduleDayRequest> Days { get; set; } = new();
    }

    public class ScheduleDayRequest
    {
        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationMinutes { get; set; }
        public bool IsActive { get; set; }
    }
}
