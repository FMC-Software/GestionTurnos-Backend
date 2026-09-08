using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Mapper
{
    public static class PlanMapper
    {
        public static Plan ToPlan(this PlanRequest request)
        {
            return new Plan
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                DurationDays = request.DurationDays,
                IsActive = request.IsActive,
                MaxStaffAllowed = request.MaxStaffAllowed,
                MaxBranchesAllowed = request.MaxBranchesAllowed,
                MaxServicesAllowed = request.MaxServicesAllowed
            };
        }

        public static void UpdateFromRequest(this Plan plan, PlanRequest request)
        {
            plan.Name = request.Name;
            plan.Description = request.Description;
            plan.Price = request.Price;
            plan.DurationDays = request.DurationDays;
            plan.IsActive = request.IsActive;
            plan.MaxStaffAllowed = request.MaxStaffAllowed;
            plan.MaxBranchesAllowed = request.MaxBranchesAllowed;
            plan.MaxServicesAllowed = request.MaxServicesAllowed;
            plan.UpdateDateTime = DateTime.Now;
        }

        public static PlanResponse ToPlanResponse(this Plan plan)
        {
            return new PlanResponse
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                Price = plan.Price,
                DurationDays = plan.DurationDays,
                IsActive = plan.IsActive,
                MaxStaffAllowed = plan.MaxStaffAllowed,
                MaxBranchesAllowed = plan.MaxBranchesAllowed,
                MaxServicesAllowed = plan.MaxServicesAllowed
            };
        }
    }
}
