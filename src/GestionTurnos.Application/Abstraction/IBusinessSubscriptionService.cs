using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction
{
    public interface IBusinessSubscriptionService
    {
        Task<List<BusinessSubscriptionResponse>> GetAll();
        Task<BusinessSubscriptionResponse> GetById(Guid id);
        Task<List<BusinessSubscriptionResponse>> GetByBusinessId(Guid businessId);
        Task<BusinessSubscriptionResponse> Create(BusinessSubscriptionRequest request);
        Task<BusinessSubscriptionResponse> UpdateStatus(Guid id, string status);
        Task Delete(Guid id);
        Task InitialBusinessSubscription(Plan plan, Business newBusiness);

        Task<BusinessSubscriptionResponse> GetCurrentSubscription(Guid businessId);

        Task RenewSubscription(Guid businessId);

        Task ChangePlan(Guid businessId, Guid planId);

        //void ChangePlan(Guid businessId, Guid newPlanId);
    }
}
