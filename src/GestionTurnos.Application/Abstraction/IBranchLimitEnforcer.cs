using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction
{
    public interface IBranchLimitEnforcer
    {
        /// Desactiva las sucursales activas que excedan el limite del plan (conserva las mas antiguas
        /// y siempre la del Admin). El personal y los turnos existentes no se modifican.
        Task ApplyBranchLimitAsync(Guid businessId, Plan plan);
    }
}
