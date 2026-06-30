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

        public bool ExistByName(Guid businessId, string name, Guid? excludeId = null)
        {
            var query = _dbSet.Where(s =>
                s.BusinessId == businessId &&
                !s.IsDeleted &&
                s.Name.Trim().ToLower() == name.Trim().ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            return query.Any();
        }

        
    }
}
