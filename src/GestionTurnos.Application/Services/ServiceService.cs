using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Abstraction.Infrastructure.External_Interface;
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
        private readonly IDolarService _dolarPriceService;

        public ServiceService(IServiceRepository serviceRepository, ITenantProvider tenantProvider, IDolarService dolarPriceService)
        {
            _serviceRepository = serviceRepository;
            _tenantProvider = tenantProvider;
            _dolarPriceService = dolarPriceService;
        }

        public async Task<List<ServiceBusinessResponse>> GetServicesOfCurrentBusiness()
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

             var Services = _serviceRepository.GetByBusinessId(businessId)
                .Where(s => !s.IsDeleted)
                .Select(s => s.ToServiceResponse())
                .ToList();

            var dolarPrice = await _dolarPriceService.CurrentDolarPrice();

            for (int i = 0; i < Services.Count; i++)
            {
                Services[i].PriceUSD = Math.Round(Services[i].Price / dolarPrice, 2);
            }

            return Services;
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
