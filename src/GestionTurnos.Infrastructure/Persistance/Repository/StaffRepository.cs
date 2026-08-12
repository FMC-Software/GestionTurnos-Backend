using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class StaffRepository : BaseRepository<Staff>, IStaffRepository
        {
        private readonly ITenantProvider _tenantProvider;
        public StaffRepository(FMCTurnosDbContext context, ITenantProvider tenantProvider) : base(context)
            {
                _tenantProvider = tenantProvider;
            }

        public async Task<List<Staff>> GetAll()
        {
            return await _dbSet.Where(s =>s.BusinessId == _tenantProvider.GetBusinessId() && !s.IsDeleted)
                .Include(s => s.Branch)
                .ToListAsync();
        }

        public async Task<Staff?> GetByEmail(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Email == email && s.BusinessId == _tenantProvider.GetBusinessId() && !s.IsDeleted);
        }
        public async Task<Staff?> GetByEmailGlobal(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);
        }

        public async Task<List<Staff>> GetByBranchId(Guid branchId)
        {
            return await _dbSet
                .Where(s => s.BranchId == branchId && !s.IsDeleted)
                .ToListAsync();
        }

        public override async Task<List<Staff>> GetAllGlobal()
        {
            return await _dbSet.Where(s => !s.IsDeleted)
                .Include(s => s.Branch)
                .Include(s => s.Business)
                .ToListAsync();
        }

        public override async Task<Staff?> GetById(Guid id)
        {
            return await _dbSet
                .Include(s => s.Business)
                .Include(s => s.Branch)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

    }


}
