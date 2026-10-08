using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction
{
    public interface IStaffLimitEnforcer
    {
        /// Desactiva el personal activo que exceda el limite del plan (conserva los mas antiguos)
        /// y deja sus turnos futuros en estado PendingReassignment.
        Task ApplyStaffLimitAsync(Guid businessId, Plan plan);
    }
}
