namespace GestionTurnos.Application.Response
{
    public class AppointmentNotificationPayload
    {
        public Guid Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Day { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid BranchId { get; set; }
        public Guid BusinessId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public string Payment { get; set; } = string.Empty;
        public string? Observation { get; set; }
    }
}