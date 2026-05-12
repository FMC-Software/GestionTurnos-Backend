using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class ClientRepository : BaseRepository<Client>, IClientRepository
    {
        public ClientRepository(FMCTurnosDbContext context) : base(context)
        {
        }
        public Client? GetClientByName(string name)
        {
            return _dbSet.FirstOrDefault(x => x.Name == name && !x.IsDeleted);
        }

        public Client? GetClientByNameForBusiness(string name, Guid businessId)
        {
            return _dbSet.FirstOrDefault(x => x.Name == name && x.BusinessId == businessId && !x.IsDeleted);
        }

        public List<Client> GetClientsOfBusiness(Guid businessId)
        {
            return _dbSet.Where(x => x.BusinessId == businessId && !x.IsDeleted).ToList();
        }
    }
}
