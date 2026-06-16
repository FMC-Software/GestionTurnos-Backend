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

        public List<Staff> GetAll()
        {
            return _dbSet.Where(s =>s.BusinessId == _tenantProvider.GetBusinessId() && !s.IsDeleted)
                .Include(s => s.Branch)
                .ToList();
        }

        public Staff GetByEmail(string email)
        {
            return _dbSet.FirstOrDefault(s => s.Email == email && s.BusinessId == _tenantProvider.GetBusinessId() && !s.IsDeleted);
        }
        public Staff GetByEmailGlobal(string email)
        {
            return _dbSet.FirstOrDefault(s => s.Email == email && !s.IsDeleted);
        }

        public override Staff? GetById(Guid id)
        {
            return _dbSet
                .Include(s => s.Business)
                .Include(s => s.Branch)
                .FirstOrDefault(s => s.Id == id && !s.IsDeleted);
        }
        
    }
       
    
}
