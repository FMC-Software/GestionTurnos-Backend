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

        public async Task<ClientsResponse> CreateClient(ClientRequest request, Guid? businessId = null)
        {
            //Si el cliente ya existe, lo retornamos sin crear uno nuevo
            var clientExisting = await _clientRepository.GetClientByEmail(request.Email, businessId) ?? null;
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

            await _clientRepository.Add(client);

            return client.ToResponse();
        }

        public async Task<List<ClientsResponse>> GetClientsOfCurrentBusiness()
        {
            var clients = await _clientRepository.GetAll();
            return clients.Select(c => c.ToResponse()).ToList();
        }

        public async Task<ClientsResponse> GetById(Guid id)
        {
            var client = await _clientRepository.GetById(id)
                ?? throw new ConflictException("Cliente no encontrado o no pertenece a su comercio.");
            return client.ToResponse();
        }

        public async Task<ClientsResponse> GetByName(string name)
        {
            var client = await _clientRepository.GetClientByName(name)
                ?? throw new ConflictException("Cliente no encontrado en su comercio.");
            return client.ToResponse();
        }

        public async Task<ClientsResponse> GetByEmail(string email)
        {
            var client = await _clientRepository.GetClientByEmail(email)
                ?? throw new ConflictException("Cliente no encontrado en su comercio.");
            return client.ToResponse();
        }

        public async Task UpdateClient(ClientRequest request, Guid id)
        {
            var existingClient = await _clientRepository.GetById(id)
                ?? throw new ConflictException("Cliente no encontrado.");


            existingClient.UpdateFromDto(request);


            await _clientRepository.Update(existingClient);
        }

        public async Task DeleteClient(Guid id)
        {
            var existingClient = await _clientRepository.GetById(id)
                ?? throw new ConflictException("Cliente no encontrado.");
            if (existingClient.IsDeleted)
            {
                throw new ConflictException("El cliente ya se encuentra eliminado.");
            }

            await _clientRepository.Delete(id);
        }


        public async Task<List<GlobalClientResponse>> GetAllGlobal()
        {
            var allClients = await _clientRepository.GetAllGlobal();

            return allClients.Select(c => c.ToGlobalResponse()).ToList();
        }
    }
}
