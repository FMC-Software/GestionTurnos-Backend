using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class BusinessRepository : BaseRepository<Business>, IBusinessRepository
    {
        public BusinessRepository(FMCTurnosDbContext context) : base(context)
        {
        }

        public List<Staff> GetAllByBusiness(Guid id_Business)
        {
            return _context.Staffs.Where(s => s.BusinessId == id_Business && !s.IsDeleted).ToList();
        }
    }
}
