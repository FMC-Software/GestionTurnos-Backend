using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction
{
    public interface IClientService 
    {
        ClientsResponse CreateClient(ClientRequest request, Guid? businessId = null);

        List<ClientsResponse> GetClientsOfCurrentBusiness();

        void UpdateClient(ClientRequest request, Guid id);

        void DeleteClient(Guid id);

        List<GlobalClientResponse> GetAllGlobal();
        ClientsResponse GetByName(string name);

        ClientsResponse GetByEmail(string email);

        ClientsResponse GetById(Guid id);


    }
}
