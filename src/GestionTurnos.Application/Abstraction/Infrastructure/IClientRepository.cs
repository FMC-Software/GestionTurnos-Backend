using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IClientRepository : IBaseRepository<Client>
    {
        Client? GetClientByName(string name);
        Client? GetClientByEmail(string email, Guid? businessId = null);

        List<Client> GetAll();

    }
}
