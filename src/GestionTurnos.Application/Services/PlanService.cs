using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class PlanService : IPlanService
    {
        private readonly IPlanRepository _planRepository;
        private readonly IBusinessSubscriptionRepository _subscriptions;

        public PlanService(IPlanRepository planRepository, IBusinessSubscriptionRepository subscriptions)
        {
            _planRepository = planRepository;
            _subscriptions = subscriptions;
        }

        public async Task<List<PlanResponse>> GetAll()
        {
            var plans = await _planRepository.GetAllGlobal();
            return plans
                .Select(p => p.ToPlanResponse())
                .ToList();
        }

        public async Task<PlanResponse> GetById(Guid id)
        {
            var plan = await _planRepository.GetById(id)
                ?? throw new ConflictException("Plan no encontrado.");

            return plan.ToPlanResponse();
        }

        public async Task<PlanResponse> Create(PlanRequest request)
        {
            var allPlans = await _planRepository.GetAllGlobal();
            bool planExist = allPlans
                .Any(p =>
                    string.Equals(
                            p.Name.Trim(),
                            request.Name.Trim(),
                            StringComparison.OrdinalIgnoreCase));

            if (planExist)
            {
                throw new ConflictException($"Ya existe un plan con el nombre '{request.Name}'");
            }

            var newPlan = request.ToPlan();

            await _planRepository.Add(newPlan);

            return newPlan.ToPlanResponse();

        }

        public async Task<PlanResponse> Update(PlanRequest request, Guid id)
        {
            var existingPlan = await _planRepository.GetById(id)
                ?? throw new ConflictException("Plan no encontrado.");

            var allPlans = await _planRepository.GetAllGlobal();
            bool duplicatedPlan = allPlans
                .Any(p =>
                    p.Id != id &&
                    string.Equals(
                        p.Name.Trim(),
                        request.Name.Trim(),
                        StringComparison.OrdinalIgnoreCase));

            if(duplicatedPlan)
            {
                throw new ConflictException($"Ya existe un plan con el nombre '{request.Name}'");
            }

            existingPlan.UpdateFromRequest(request);

            await _planRepository.Update(existingPlan);

            return existingPlan.ToPlanResponse();
        }

        public async Task Delete(Guid id)
        {
            var plan = await _planRepository.GetById(id)
                ?? throw new ConflictException("Plan no encontrado.");

            if (string.Equals(
                plan.Name,
                "Free Plan",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new ConflictException("No se puede eliminar el plan por defecto");
            }

            var allSubscriptions = await _subscriptions.GetAllGlobal();
            bool hasSubscriptions = allSubscriptions
                    .Any(s => s.PlanId == id);

            if (hasSubscriptions)
            {
                throw new ConflictException("No se puede eliminar un plan que esta siendo utilizado.");
            }

            await _planRepository.Delete(id);
        }

        public async Task<Plan> GetPlanOrDefault(Guid? planId)
        {
            if (!planId.HasValue || planId == Guid.Empty)
            {
                var allPlans = await _planRepository.GetAllGlobal();
                return allPlans.FirstOrDefault(p => p.Name == "Free Plan")
                    ?? allPlans.FirstOrDefault()
                    ?? throw new ConflictException("No se encontro plan.");
            }

            return await _planRepository.GetById(planId.Value)
                ?? throw new ConflictException("El plan especificado no existe");
        }

        public async Task<Plan> GetActivePlan(Guid planId)
        {
            var plan = await _planRepository.GetById(planId)
                ?? throw new NotFoundException("El plan especificado no existe");
            if (!plan.IsActive)
            {
                throw new ConflictException("El plan se encuentra inactivo");
            }

            return plan;
        }
    }
}
