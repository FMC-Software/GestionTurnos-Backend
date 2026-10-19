namespace GestionTurnos.Application.Response
{
    public class PendingReassignmentResponse : AppointmentResponse
    {
        public Guid StaffId { get; set; }
        public string StaffRol { get; set; } = string.Empty;
        public bool StaffIsActive { get; set; }

        public Guid BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;

        public Guid ClientId { get; set; }
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;

        public Guid ServiceId { get; set; }
        public int ServiceDuration { get; set; }
    }
}
