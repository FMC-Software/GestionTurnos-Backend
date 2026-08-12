using GestionTurnos.Application.Json;
using GestionTurnos.Domain.Entities;
using System.Text.Json.Serialization;

namespace GestionTurnos.Application.Request
{
    public class AppointmentRequest
    {
        // Datos del turno
        public Guid StaffId { get; set; }
        public Guid BranchId { get; set; }
        public Guid ServiceId { get; set; }
        public DateTime Day { get; set; }

        [JsonConverter(typeof(FlexibleTimeSpanJsonConverter))]
        public TimeSpan StartTime { get; set; }
        public string? Observation { get; set; }
        public PaymentMethod Payment { get; set; }

        // Datos del cliente (se usa para buscar o crear)
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public DateTime ClientBirthDay { get; set; }
    }
}
