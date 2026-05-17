using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class BusinessRepository : BaseRepository<Business>, IBusinessRepository
    {
        public BusinessRepository(FMCTurnosDbContext context) : base(context)
        {
        }

        public List<Business> GetAllByBusiness(Guid id_Business)
        {
            return _dbSet.Where(s => s.Id == id_Business && !s.IsDeleted).ToList();
        }
    }
}
