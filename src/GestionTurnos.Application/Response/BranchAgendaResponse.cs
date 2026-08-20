namespace GestionTurnos.Application.Response
{
    public class BranchAgendaResponse
    {
        public Guid BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public ScheduleInfoResponse? Schedule { get; set; }
        public List<StaffAgendaResponse> Staff { get; set; } = new();
    }

    public class ScheduleInfoResponse
    {
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int SlotDurationMinutes { get; set; }
    }

    public class StaffAgendaResponse
    {
        public Guid StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public List<AgendaAppointmentResponse> Appointments { get; set; } = new();
    }

    public class AgendaAppointmentResponse
    {
        public Guid Id { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
