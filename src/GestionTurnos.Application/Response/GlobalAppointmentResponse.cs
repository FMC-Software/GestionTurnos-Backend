namespace GestionTurnos.Application.Response
{
    public class GlobalAppointmentResponse : AppointmentResponse
    {
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
    }
}
