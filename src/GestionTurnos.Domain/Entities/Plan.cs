using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GestionTurnos.Domain.Entities
{
    public class Plan : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        //[Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int DurationDays { get; set; }

        public bool IsActive { get; set; } = true;

        public int MaxStaffAllowed { get; set; } = -1;
        public int MaxBranchesAllowed { get; set; } = -1;
        public int MaxServicesAllowed { get; set; } = -1;

        // Propiedad de navegación inversa
        public virtual ICollection<BusinessSubscription> Subscriptions { get; set; } = new List<BusinessSubscription>();
    }
}
