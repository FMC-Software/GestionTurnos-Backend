using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;

namespace GestionTurnos.Application.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ITenantProvider _tenantProvider;

        public ServiceService(IServiceRepository serviceRepository, ITenantProvider tenantProvider)
        {
            _serviceRepository = serviceRepository;
            _tenantProvider = tenantProvider;
        }

        public List<ServiceBusinessResponse> GetServicesOfCurrentBusiness()
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            return _serviceRepository.GetByBusinessId(businessId)
                .Where(s => !s.IsDeleted)
                .Select(s => s.ToServiceResponse())
                .ToList();
        }

        public ServiceBusinessResponse GetById(Guid id)
        {
            var service = _serviceRepository.GetById(id)
                ?? throw new ConflictException("Servicio no encontrado.");

            return service.ToServiceResponse();
        }

        public ServiceBusinessResponse CreateService(ServiceRequest request)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var newService = request.ToService(businessId);

            _serviceRepository.Add(newService);

            return newService.ToServiceResponse();
        }

        public ServiceBusinessResponse UpdateService(ServiceRequest request, Guid id)
        {
            var existingService = _serviceRepository.GetById(id)
                ?? throw new ConflictException("Servicio no encontrado.");

            existingService.UpdateFromRequest(request);

            _serviceRepository.Update(existingService);

            return existingService.ToServiceResponse();
        }

        public void DeleteService(Guid id)
        {
            var service = _serviceRepository.GetById(id)
                ?? throw new ConflictException("Servicio no encontrado.");

            _serviceRepository.Delete(id);
        }
    }
}
