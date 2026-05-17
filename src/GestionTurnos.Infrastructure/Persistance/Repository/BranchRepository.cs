using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class BranchRepository : BaseRepository<Branch>, IBranchRepository
    {
        public BranchRepository(FMCTurnosDbContext context) : base(context)
        {
        }
    }
}
