using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GestionTurnos.Domain.Entities
{
    public enum Status
    {
        Active,
        Inactive,
        Cancelled,
        Expired
    }
    public class BusinessSubscription : BaseEntity
    {   
        public Guid BusinessId { get; set; }
        public Business Business { get; set; } = null!;
        public Guid PlanId { get; set; }
        public Plan Plan { get; set; } = null!;

        // Plan al que pasara el negocio cuando finalice esta suscripcion (downgrade programado).
        public Guid? PendingPlanId { get; set; }
        public Plan? PendingPlan { get; set; }


        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Status Status { get; set; }
    }
}
