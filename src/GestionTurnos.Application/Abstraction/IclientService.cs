using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction
{
    public interface IClientService
    {
        Task<ClientsResponse> CreateClient(ClientRequest request, Guid? businessId = null);

        Task<List<ClientsResponse>> GetClientsOfCurrentBusiness();

        Task UpdateClient(ClientRequest request, Guid id);

        Task DeleteClient(Guid id);

        Task<List<GlobalClientResponse>> GetAllGlobal();
        Task<ClientsResponse> GetByName(string name);

        Task<ClientsResponse> GetByEmail(string email);

        Task<ClientsResponse> GetById(Guid id);


    }
}
