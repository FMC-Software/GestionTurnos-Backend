using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using BCrypt.Net;

namespace GestionTurnos.Domain.Entities
{

    public enum Rol { 
        
        Admin, // Se encarga de la administración del sistema.
        Recepcionista, // Se encarga del cobro, y la gestion de todos los turnos de cualquier profesional(Puede agregar turnos ).
        Profesional // Se encarga de gestionar sus propios turnos, y de atender a los clientes.
    }
    public class Staff : User
    {
        [Required]
        public Guid BusinessId { get; set; }
        public Business Business { get; set; } = null!;
        public Guid BranchId { get; set; }
        public  Branch Branch { get; set; } = null;

        public string LinkPhoto { get; set; } = string.Empty;

        public Rol Rol { get; set; }

        // False cuando el plan del negocio no permite mas personal activo. No se reactiva automaticamente.
        public bool IsActive { get; set; } = true;

        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

        // Propiedad de navegación inversa: Un profesional tiene muchos turnos
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    }
}
