using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionTurnos.Domain.Entities
{
    public class Service : BaseEntity
    {
        [Required]
        public Guid BusinessId { get; set; }
        public Business Business { get; set; } = null!;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Duration { get; set; }

        public double Price { get; set; }

        // Propiedad de navegación inversa
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
