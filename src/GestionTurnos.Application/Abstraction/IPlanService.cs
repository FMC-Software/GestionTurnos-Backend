using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction
{
    public interface IPlanService
    {
        Task<List<PlanResponse>> GetAll();
        Task<PlanResponse> GetById(Guid id);
        Task<PlanResponse> Create(PlanRequest request);
        Task<PlanResponse> Update(PlanRequest request, Guid id);
        Task Delete(Guid id);

        Task<Plan> GetPlanOrDefault(Guid? planId);

        Task<Plan> GetActivePlan(Guid PlanId);
    }
}
