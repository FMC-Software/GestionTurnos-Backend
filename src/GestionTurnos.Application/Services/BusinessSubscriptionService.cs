using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class BusinessSubscriptionService : IBusinessSubscriptionService
    {
        private readonly IBusinessSubscriptionRepository _subscriptionRepository;
        private readonly IPlanService _planService;

        public BusinessSubscriptionService(IBusinessSubscriptionRepository subscriptionRepository, IPlanService planService)
        {
            _subscriptionRepository = subscriptionRepository;
            _planService = planService;
        }

        private async Task<BusinessSubscription> GetCurrentSubscriptionEntity(Guid businessId)
        {
            return await _subscriptionRepository
                .GetCurrentSubscription(businessId)
                ?? throw new ConflictException("El negocio no posee una suscripcion activa");
        }

        public async Task<List<BusinessSubscriptionResponse>> GetAll()
        {
            var subscriptions = await _subscriptionRepository.GetAllWithDetails();
            return subscriptions
                .Select(s => s.ToBusinessSubscriptionResponse())
                .ToList();
        }

        public async Task<BusinessSubscriptionResponse> GetById(Guid id)
        {
            var subscription = await _subscriptionRepository.GetByIdWithDetails(id)
                ?? throw new ConflictException("Suscripción no encontrada.");

            return subscription.ToBusinessSubscriptionResponse();
        }

        public async Task<List<BusinessSubscriptionResponse>> GetByBusinessId(Guid businessId)
        {
            var subscriptions = await _subscriptionRepository.GetByBusinessId(businessId);
            return subscriptions
                .Select(s => s.ToBusinessSubscriptionResponse())
                .ToList();
        }

        public async Task<BusinessSubscriptionResponse> Create(BusinessSubscriptionRequest request)
        {
            var newSubscription = request.ToBusinessSubscription();
            await _subscriptionRepository.Add(newSubscription);

            // Reload with details so the response has Business/Plan names
            var created = await _subscriptionRepository.GetByIdWithDetails(newSubscription.Id)
                ?? throw new ConflictException("Error al obtener la suscripción creada.");

            return created.ToBusinessSubscriptionResponse();
        }

        public async Task<BusinessSubscriptionResponse> UpdateStatus(Guid id, string status)
        {
            if (!Enum.TryParse<Status>(status, ignoreCase: true, out var parsedStatus))
                throw new ConflictException($"Estado inválido: '{status}'. Los valores permitidos son: {string.Join(", ", Enum.GetNames<Status>())}.");

            var subscription = await _subscriptionRepository.GetByIdWithDetails(id)
                ?? throw new ConflictException("Suscripción no encontrada.");

            if (parsedStatus == Status.Expired)
            {
                throw new ConflictException("El estado Expired es administrado automaticamente por el sistema");
            }

            subscription.Status = parsedStatus;

            await _subscriptionRepository.Update(subscription);

            return subscription.ToBusinessSubscriptionResponse();
        }

        public async Task Delete(Guid id)
        {
            var subscription = await _subscriptionRepository.GetById(id)
                ?? throw new ConflictException("Suscripción no encontrada.");

            await _subscriptionRepository.Delete(id);
        }


        public async Task InitialBusinessSubscription(Plan plan, Business newBusiness)
        {
            var BusinessSubscription = new BusinessSubscription
            {
                Id = Guid.NewGuid(),
                BusinessId = newBusiness.Id,
                Business = newBusiness,
                PlanId = plan.Id,
                Plan = plan,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow + TimeSpan.FromDays(plan.DurationDays),
                Status = Status.Active
            };
            await _subscriptionRepository.Add(BusinessSubscription);
        }


        public async Task<BusinessSubscriptionResponse> GetCurrentSubscription(Guid businessId)
        {
            var subscription = await GetCurrentSubscriptionEntity(businessId);
            return subscription.ToBusinessSubscriptionResponse();
        }

        public async Task RenewSubscription(Guid businessId)
        {
            var subscription = await _subscriptionRepository
                .GetLatestByBusinessId(businessId)
                ?? throw new ConflictException("El negocio no posee suscripciones");

            if(subscription.Status == Status.Cancelled)
            {
                throw new ConflictException("No se puede renovar una suscripcion cancelada");
            }

            if (subscription.Plan == null)
            {
                throw new ConflictException("La suscripción no tiene un plan asociado.");
            }

            //logica para que si ya pago el mes y quiere renovar antes que termine, no pierda los dias, sino que extienda
            if(subscription.EndDate > DateTime.UtcNow)
            {
                subscription.EndDate = subscription.EndDate.AddDays(subscription.Plan.DurationDays);
            }
            else
            {
                subscription.StartDate = DateTime.UtcNow;
                subscription.EndDate = DateTime.UtcNow.AddDays(subscription.Plan.DurationDays);
            }

            subscription.Status = Status.Active;

            await _subscriptionRepository.Update(subscription);
        }

        public async Task ChangePlan(Guid businessId, Guid newPlanId)
        {
            var currentSubscription = await _subscriptionRepository
                .GetLatestByBusinessId(businessId)
                ?? throw new NotFoundException("El negocio no posee suscripciones");

            var newPlan = await _planService.GetActivePlan(newPlanId);

            if(currentSubscription.PlanId == newPlan.Id)
            {
                throw new ConflictException("El negocio ya posee este plan");
            }

            currentSubscription.Status = Status.Inactive;

            await _subscriptionRepository.Update(currentSubscription);

            var newSubscription = new BusinessSubscription
            {
                BusinessId = businessId,
                PlanId = newPlan.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(newPlan.DurationDays),
                Status = Status.Active
            };

            await _subscriptionRepository.Add(newSubscription);

        }
    }
}
