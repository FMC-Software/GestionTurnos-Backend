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

        public List<PlanResponse> GetAll()
        {
            return _planRepository.GetAllGlobal()
                .Select(p => p.ToPlanResponse())
                .ToList();
        }

        public PlanResponse GetById(Guid id)
        {
            var plan = _planRepository.GetById(id)
                ?? throw new ConflictException("Plan no encontrado.");

            return plan.ToPlanResponse();
        }

        public PlanResponse Create(PlanRequest request)
        {
            bool planExist = _planRepository
                .GetAllGlobal()
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

            _planRepository.Add(newPlan);

            return newPlan.ToPlanResponse();

        }

        public PlanResponse Update(PlanRequest request, Guid id)
        {
            var existingPlan = _planRepository.GetById(id)
                ?? throw new ConflictException("Plan no encontrado.");

            bool duplicatedPlan = _planRepository
                .GetAllGlobal()
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

            _planRepository.Update(existingPlan);

            return existingPlan.ToPlanResponse();
        }

        public void Delete(Guid id)
        {
            var plan = _planRepository.GetById(id)
                ?? throw new ConflictException("Plan no encontrado.");

            if (string.Equals(
                plan.Name,
                "Free Plan",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new ConflictException("No se puede eliminar el plan por defecto");
            }

            bool hasSubscriptions = _subscriptions
                    .GetAllGlobal()
                    .Any(s => s.PlanId == id);

            if (hasSubscriptions) 
            { 
                throw new ConflictException("No se puede eliminar un plan que esta siendo utilizado.");
            }

            _planRepository.Delete(id);
        }

        public Plan GetPlanOrDefault(Guid? planId)
        {
            if (!planId.HasValue || planId == Guid.Empty)
            {
                return _planRepository.GetAllGlobal()
                    .FirstOrDefault(p => p.Name == "Free Plan")
                    ?? _planRepository.GetAllGlobal().FirstOrDefault()
                    ?? throw new ConflictException("No se encontro plan.");
            }

            return _planRepository.GetById(planId.Value)
                ?? throw new ConflictException("El plan especificado no existe");
        }
    }
}
