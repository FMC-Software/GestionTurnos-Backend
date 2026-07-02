namespace GestionTurnos.Application.Response
{
    public class AppointmentResponse
    {
        public Guid Id { get; set; }

        public string ClientName { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;

        public string Day { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;

        public string? Observation { get; set; }

        public string Payment { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public decimal TotalCost { get; set; }
    }
}