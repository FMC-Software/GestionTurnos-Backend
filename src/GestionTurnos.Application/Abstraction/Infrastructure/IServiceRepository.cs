using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IServiceRepository : IBaseRepository<Service>
    {
        List<Service> GetByBusinessId(Guid businessId);
    }
}
