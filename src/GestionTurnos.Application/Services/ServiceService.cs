using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Abstraction.Infrastructure.External_Interface;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Helpers;
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
        private readonly IBusinessSubscriptionService _businessSubscriptionService;

        public ServiceService(IServiceRepository serviceRepository, ITenantProvider tenantProvider, IDolarService dolarPriceService, IBusinessSubscriptionService businessSubscriptionService)
        {
            _serviceRepository = serviceRepository;
            _tenantProvider = tenantProvider;
            _dolarPriceService = dolarPriceService;
            _businessSubscriptionService = businessSubscriptionService;
        }

        public async Task<List<ServiceBusinessResponse>> GetServicesOfCurrentBusiness()
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var servicesEntities = await _serviceRepository.GetByBusinessId(businessId);
            var Services = servicesEntities
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

        public async Task<List<ServiceResponse>> GetServicesByBusinessId(Guid businessId)
        {
            var services = await _serviceRepository.GetByBusinessId(businessId);
            return services.Select(s => s.ToResponse()).ToList();
        }

        public async Task<ServiceBusinessResponse> GetById(Guid id)
        {
            var service = await _serviceRepository.GetById(id)
                ?? throw new ConflictException("Servicio no encontrado.");

            return service.ToServiceResponse();
        }

        public async Task<ServiceBusinessResponse> CreateService(ServiceRequest request)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            await ValidateService(request, businessId);

            var existingServices = await _serviceRepository.GetByBusinessId(businessId);
            var plan = await _businessSubscriptionService.GetActivePlanForBusiness(businessId);
            PlanLimitGuard.EnsureWithinLimit(existingServices.Count, plan.MaxServicesAllowed, "servicios");

            var newService = request.ToService(businessId);

            await _serviceRepository.Add(newService);

            return newService.ToServiceResponse();
        }

        public async Task<ServiceBusinessResponse> UpdateService(ServiceRequest request, Guid id)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontro la empresa");

            var existingService = await _serviceRepository.GetById(id)
                ?? throw new ConflictException("Servicio no encontrado.");

            await ValidateService(request, businessId, id);

            existingService.UpdateFromRequest(request);

            await _serviceRepository.Update(existingService);

            return existingService.ToServiceResponse();
        }

        public async Task DeleteService(Guid id)
        {
            var service = await _serviceRepository.GetById(id)
                ?? throw new ConflictException("Servicio no encontrado.");

            await _serviceRepository.Delete(id);
        }

        private async Task ValidateService(ServiceRequest request, Guid businessId, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ConflictException("Debe ingresar un nombre");
            }

            if (request.Price <= 0)
            {
                throw new ConflictException("El precio debe ser mayor a 0");
            }

            if (request.Duration <= 0)
            {
                throw new ConflictException("La duracion del servicio debe ser mayor a 0");
            }

            if(await _serviceRepository.ExistByName(businessId, request.Name, excludeId))
            {
                throw new ConflictException("Ya existe un servicio con ese nombre");
            }
        }
    }
}
