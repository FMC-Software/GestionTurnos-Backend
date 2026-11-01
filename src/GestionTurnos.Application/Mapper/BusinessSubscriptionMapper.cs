using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Mapper
{
    public static class BusinessSubscriptionMapper
    {
        public static BusinessSubscription ToBusinessSubscription(this BusinessSubscriptionRequest request)
        {
            return new BusinessSubscription
            {
                BusinessId = request.BusinessId,
                PlanId = request.PlanId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = Status.Active
            };
        }

        public static BusinessSubscriptionResponse ToBusinessSubscriptionResponse(this BusinessSubscription subscription)
        {
            return new BusinessSubscriptionResponse
            {
                Id = subscription.Id,
                BusinessId = subscription.BusinessId,
                BusinessName = subscription.Business?.Name ?? string.Empty,
                PlanId = subscription.PlanId,
                PlanName = subscription.Plan?.Name ?? string.Empty,
                PendingPlanId = subscription.PendingPlanId,
                PendingPlanName = subscription.PendingPlan?.Name,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                Status = subscription.Status.ToString()
            };
        }
    }
}
