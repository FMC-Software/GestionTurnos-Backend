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
        public string Phone { get; set; }
        public string? City { get; set; }
    }

}
