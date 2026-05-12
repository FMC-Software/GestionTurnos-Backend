using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        public Client CreateClient(Client client)
        {
            return _clientRepository.Add(client);
        }

        public void DeleteClient(Guid id)
        {
             _clientRepository.Delete(id);
        }

        public List<Client> GetAll()
        {
            return _clientRepository.GetAll();
        }

        public Client GetById(Guid id)
        {
            var existingClient = _clientRepository.GetById(id) ?? throw new Exception("Cliente no encontrado");
            return existingClient;
        }

        public Client GetByName(string name)
        {
            var existingClient = _clientRepository.GetClientByName(name) ?? throw new Exception("Cliente no encontrado");
            return existingClient;
        }

        public Client GetByNameForBusiness(string name, Guid businessId)
        {
            var existingClient = _clientRepository.GetClientByNameForBusiness(name,businessId) ?? throw new Exception("Cliente no encontrado");
            return existingClient;
        }

        public List<Client> GetClientsOfBusiness(Guid businessId)
        {
           return _clientRepository.GetClientsOfBusiness(businessId);
        }

        public void UpdateClient(Client client)
        {
            var existingClient = _clientRepository.GetById(client.Id) ?? throw new Exception("Cliente no encontrado");
            _clientRepository.Update(client);
        }
    }
}
