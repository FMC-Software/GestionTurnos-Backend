using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class ClientRepository : BaseRepository<Client>, IClientRepository
    {
        private readonly ITenantProvider _tenantProvider;
        public ClientRepository(FMCTurnosDbContext context, ITenantProvider tenantProvider) : base(context)
        {
            _tenantProvider = tenantProvider;
        }
        public async Task<Client?> GetClientByName(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Name.Contains(name) && x.BusinessId == _tenantProvider.GetBusinessId() && !x.IsDeleted);
        }

        public async Task<Client?> GetClientByEmail(string email, Guid? businessId = null)
        {
            var bId = businessId ?? _tenantProvider.GetBusinessId();
            return await _dbSet.FirstOrDefaultAsync(x => x.Email == email && x.BusinessId == bId && !x.IsDeleted);
        }


        public override async Task<List<Client>> GetAllGlobal()
        {
            return await _context.Clients
                           .IgnoreQueryFilters()
                           .Where(x => !x.IsDeleted)
                           .Include(x => x.Business)
                           .ToListAsync();
        }

        public async Task<List<Client>> GetAll()
        {
            var businessId = _tenantProvider.GetBusinessId();
            if (businessId == null)
            {
                throw new ConflictException("No se encontró la empresa.");
            }
            return await _dbSet.Where(x => x.BusinessId == businessId && !x.IsDeleted).ToListAsync();
        }
    }
}
