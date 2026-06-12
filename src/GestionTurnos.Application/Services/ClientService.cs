using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly ITenantProvider _tenantProvider;

        public ClientService(IClientRepository clientRepository, ITenantProvider tenantProvider)
        {
            _clientRepository = clientRepository;
            _tenantProvider = tenantProvider;
        }

        public ClientsResponse CreateClient(ClientRequest request, Guid? businessId = null)
        {
            //Si el cliente ya existe, lo retornamos sin crear uno nuevo
            var clientExisting = _clientRepository.GetClientByEmail(request.Email) ?? null;
               if(clientExisting is not null) return clientExisting.ToResponse();

            //Si el cliente no existe, lo creamos
            var client = request.ToEntity(); // Mapper

            // Si llega un businessId, lo asignamos(Esto si lo hace el client propio). Si no, lo obtenemos del tenant provider (Esto si lo hace un admin o un empleado).

            client.BusinessId = businessId ?? _tenantProvider.GetBusinessId() 
                ?? throw new ConflictException("No se encontró la empresa.");
                
            if (DateTime.TryParse(request.BirthDay, out DateTime parsedDate))
            {
                client.BirthDay = parsedDate;
            }
            
            client.UpdateDateTime = DateTime.UtcNow;

            _clientRepository.Add(client);

            return client.ToResponse();
        }

        public List<ClientsResponse> GetClientsOfCurrentBusiness()
        {
            var clients = _clientRepository.GetAllGlobal();
            return clients.Select(c => c.ToResponse()).ToList();
        }

        public ClientsResponse GetById(Guid id)
        {
            var client = _clientRepository.GetById(id)
                ?? throw new ConflictException("Cliente no encontrado o no pertenece a su comercio.");
            return client.ToResponse();
        }

        public ClientsResponse GetByName(string name)
        {
            var client = _clientRepository.GetClientByName(name)
                ?? throw new ConflictException("Cliente no encontrado en su comercio.");
            return client.ToResponse();
        }

        public ClientsResponse GetByEmail(string email)
        {
            var client = _clientRepository.GetClientByEmail(email)
                ?? throw new ConflictException("Cliente no encontrado en su comercio.");
            return client.ToResponse();
        }

        public void UpdateClient(ClientRequest request, Guid id)
        {
            var existingClient = _clientRepository.GetById(id)
                ?? throw new ConflictException("Cliente no encontrado.");

            
            existingClient.UpdateFromDto(request);

            
            _clientRepository.Update(existingClient);
        }

        public void DeleteClient(Guid id)
        {
            var existingClient = _clientRepository.GetById(id)
                ?? throw new ConflictException("Cliente no encontrado.");
            if (existingClient.IsDeleted)
            {
                throw new ConflictException("El cliente ya se encuentra eliminado.");
            }   

            _clientRepository.Delete(id);
        }


        public List<GlobalClientResponse> GetAllGlobal()
        {
            var allClients = _clientRepository.GetAllGlobal();
  
            return allClients.Select(c => c.ToGlobalResponse()).ToList();
        }
    }
}