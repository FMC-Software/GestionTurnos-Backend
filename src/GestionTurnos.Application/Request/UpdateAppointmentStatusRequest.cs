using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Request
{
    public class UpdateAppointmentStatusRequest
    {
        public AppointmentStatus Status { get; set; }
    }
}
