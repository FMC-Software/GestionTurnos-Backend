using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class BranchRepository : BaseRepository<Branch>, IBranchRepository
    {
        public BranchRepository(FMCTurnosDbContext context) : base(context)
        {
        }

        public override List<Branch> GetAllGlobal()
        {
            return _context.Branches
                .Where(x => !x.IsDeleted)
                .ToList();
        }

        public List<Branch> GetByBusinessId(Guid businessId)
        {
            return _context.Branches
                .Where(x => x.BusinessId == businessId)
                .ToList();
        }
    }
}
