using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace GestionTurnos.Domain.Entities
{

    public enum Rol { Sysadmin,
        Admin, // Se encarga de la administración del sistema.
        Recepcionista, // Se encarga del cobro, y la gestion de todos los turnos de cualquier profesional(Puede agregar turnos ).
        Profesional // Se encarga de gestionar sus propios turnos, y de atender a los clientes.
    }
    public class Staff : User
    {
        [Required]
        public Guid BusinessId { get; set; }
        public Business Business { get; set; } = null!;
        public Guid? BranchId { get; set; }
        public  Branch? Branch { get; set; } = null;
      
        public string Password { get; set; } = string.Empty;

        public string LinkPhoto { get; set; } = string.Empty;

        public Rol Rol { get; set; }

        // Propiedad de navegación inversa: Un profesional tiene muchos turnos
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    }
}
