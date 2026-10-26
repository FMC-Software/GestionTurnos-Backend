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
        //[Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // False cuando el plan del negocio no permite mas servicios activos. No se reactiva automaticamente.
        public bool IsActive { get; set; } = true;

        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

        // Propiedad de navegación inversa
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
