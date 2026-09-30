using System.ComponentModel.DataAnnotations.Schema;

namespace GestionTurnos.Domain.Entities
{
    public enum AppointmentStatus
    {
        Pending, // Apenas se crea el turno
        Confirmed, // El cliente paso, tomo el sevicio, pago y se fue
        Cancelled, // El cliente cancelo el turno o no se presento
        PendingReassignment // El profesional fue desactivado por limite de plan; el admin debe reasignar el turno
    }

    public enum PaymentMethod
    {
        Cash,
        MercadoPago,
        Transfer
    }
    public class Appointment : BaseEntity
    {
        public Guid StaffId { get; set; }
        public Staff Staff { get; set; } = null!;


        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;

    
        public Guid ServiceId { get; set; }
        public Service Service { get; set; } = null!;


        public DateTime Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Observation { get; set; }
        public PaymentMethod Payment { get; set; } 
        public AppointmentStatus Status { get; set; }

        //[Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }
    }
}
