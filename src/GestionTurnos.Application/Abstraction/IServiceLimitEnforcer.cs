using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction
{
    public interface IServiceLimitEnforcer
    {
        /// Desactiva los servicios activos que excedan el limite del plan (conserva los mas antiguos).
        /// Los turnos ya reservados no se modifican.
        Task ApplyServiceLimitAsync(Guid businessId, Plan plan);
    }
}
