using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class ServiceRepository : BaseRepository<Service>, IServiceRepository
    {
        public ServiceRepository(FMCTurnosDbContext context) : base(context)
        {
        }

        public List<Service> GetByBusinessId(Guid businessId)
        {
            return _dbSet
                .Where(s => s.BusinessId == businessId && !s.IsDeleted)
                .ToList();
        }
    }
}
