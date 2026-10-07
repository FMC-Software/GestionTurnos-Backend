using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IClientRepository : IBaseRepository<Client>
    {
        Task<Client?> GetClientByEmail(string email, Guid? businessId = null);

        Task<List<Client>> GetAll();

        Task<List<Client>> SearchClients(string query, Guid businessId);

    }
}
