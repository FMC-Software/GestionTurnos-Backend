using System.ComponentModel.DataAnnotations;

namespace GestionTurnos.Application.Request
{
    public class CancelAppointmentRequest
    {
        // Email con el que se reservo el turno; sirve para verificar a quien cancela.
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
