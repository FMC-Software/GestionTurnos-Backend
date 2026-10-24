using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class ServiceLimitEnforcer : IServiceLimitEnforcer
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceLimitEnforcer(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task ApplyServiceLimitAsync(Guid businessId, Plan plan)
        {
            if (plan.MaxServicesAllowed < 0)
                return;

            var activeServices = (await _serviceRepository.GetByBusinessId(businessId))
                .Where(s => s.IsActive)
                .ToList();

            if (activeServices.Count <= plan.MaxServicesAllowed)
                return;

            var toDeactivate = activeServices
                .OrderBy(s => s.CreatedDateTime)
                .Skip(plan.MaxServicesAllowed);

            foreach (var service in toDeactivate)
            {
                service.IsActive = false;
                service.UpdateDateTime = DateTime.UtcNow;
                await _serviceRepository.Update(service);
            }
        }
    }
}
