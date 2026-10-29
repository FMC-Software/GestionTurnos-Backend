using System.ComponentModel.DataAnnotations;

namespace GestionTurnos.Application.Request
{
    public class PlanReorderRequest
    {
        // Ids de todos los planes, en el orden deseado (el primero queda con
        // DisplayOrder 0, el segundo 1, etc.).
        [Required]
        public List<Guid> PlanIds { get; set; } = new();
    }
}
