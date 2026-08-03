using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
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

        public override async Task<List<Branch>> GetAllGlobal()
        {
            return await _context.Branches
                .Where(x => !x.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Branch>> GetByBusinessId(Guid businessId)
        {
            return await _context.Branches
                .Where(x => x.BusinessId == businessId)
                .ToListAsync();
        }

        public async Task<Branch?> GetInfoBranch( Guid branchId)
        {
            return await _dbSet

                .Include(b => b.Schedules.Where(s => !s.IsDeleted))

                .Include(b => b.Staff.Where(s => !s.IsDeleted))

                .Include(b => b.Services.Where(s => !s.IsDeleted))
                .FirstOrDefaultAsync(b => b.Id == branchId && !b.IsDeleted);
        }

        public override async Task Delete(Guid id)
        {
            var entity = await _dbSet
                .Include(b => b.Staff)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity.IsDeleted)
                throw new ConflictException("El registro ya se encuentra eliminado.");

            if (entity.Staff.Any(s => !s.IsDeleted && s.Rol == Rol.Admin))
                throw new ConflictException("No se puede eliminar una sucursal que tiene un administrador asignado.");

            entity.IsDeleted = true;
            entity.DeleteDateTime = DateTime.UtcNow;
            entity.UpdateDateTime = DateTime.UtcNow;
            _dbSet.Update(entity);
            await SaveChanges();
        }

    }
}
