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
        public Client? GetClientByName(string name)
        {
            return _dbSet.FirstOrDefault(x => x.Name.Contains(name) && x.BusinessId == _tenantProvider.GetBusinessId() && !x.IsDeleted);
        }

        public Client? GetClientByEmail(string email, Guid? businessId = null)
        {
            var bId = businessId ?? _tenantProvider.GetBusinessId();
            return _dbSet.FirstOrDefault(x => x.Email == email && x.BusinessId == bId && !x.IsDeleted);
        }


        public override List<Client> GetAllGlobal()
        {
            return _context.Clients
                           .IgnoreQueryFilters() 
                           .Where(x => !x.IsDeleted)
                           .Include(x => x.Business) 
                           .ToList();
        }

        public List<Client> GetAll()
        {
            var businessId = _tenantProvider.GetBusinessId();
            if (businessId == null)
            {
                throw new ConflictException("No se encontró la empresa.");
            }
            return _dbSet.Where(x => x.BusinessId == businessId && !x.IsDeleted).ToList();
        }
    }
}
