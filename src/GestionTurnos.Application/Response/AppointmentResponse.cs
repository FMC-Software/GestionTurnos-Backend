namespace GestionTurnos.Application.Response
{
    public class AppointmentResponse
    {
        public Guid Id { get; set; }

        public string ClientName { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;

        public DateTime Day { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string? Observation { get; set; }

        public string Payment { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public decimal TotalCost { get; set; }
    }
}