using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

        public Branch? GetInfoBranch( Guid branchId)
        {
            return _dbSet
    
                .Include(b => b.Schedules.Where(s => !s.IsDeleted))
                
                .Include(b => b.Staff.Where(s => !s.IsDeleted))
                
                .Include(b => b.Services.Where(s => !s.IsDeleted))
                .FirstOrDefault(b => b.Id == branchId && !b.IsDeleted);
        }
    }
}
