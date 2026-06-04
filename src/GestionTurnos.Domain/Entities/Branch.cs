using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionTurnos.Domain.Entities
{
    public class Branch : BaseEntity
    {
        [Required]
        public Guid BusinessId { get; set; }

        public  Business Business { get; set; } = null!;

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? City { get; set; }

        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

        // Una sucursal tiene muchos empleados (Staff)
        public ICollection<Staff> Staff { get; set; } = new List<Staff>();

        // Una sucursal ofrece muchos servicios
        // (Si es Many-to-Many directo en EF Core, se declara así. 
        // Si usas una tabla intermedia manual, acá iría la colección a esa tabla intermedia).
        public ICollection<Service> Services { get; set; } = new List<Service>();
    }

}
