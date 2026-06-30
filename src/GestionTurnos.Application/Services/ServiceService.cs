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

            ValidateService(request, businessId);
        

        var newService = request.ToService(businessId);

            _serviceRepository.Add(newService);

            return newService.ToServiceResponse();
        }

        public ServiceBusinessResponse UpdateService(ServiceRequest request, Guid id)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontro la empresa");

            var existingService = _serviceRepository.GetById(id)
                ?? throw new ConflictException("Servicio no encontrado.");

            ValidateService(request, businessId, id);

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

        private void ValidateService(ServiceRequest request, Guid businessId, Guid? excludeId = null)
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

            if(_serviceRepository.ExistByName(businessId, request.Name, excludeId))
            {
                throw new ConflictException("Ya existe un servicio con ese nombre");
            }
        }
    }
}
