using System.ComponentModel.DataAnnotations;

namespace GestionTurnos.Application.Request
{
    public class ChangeSubscriptionPlanRequest
    {
        [Required]
        public Guid PlanId { get; set; }
    }
}
