using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IBusinessSubscriptionRepository : IBaseRepository<BusinessSubscription>
    {
        Task<List<BusinessSubscription>> GetActiveSubscriptionsAsync();
        Task UpdateAsync(BusinessSubscription entity);
        Task<List<BusinessSubscription>> GetAllWithDetails();
        Task<BusinessSubscription?> GetByIdWithDetails(Guid id);
        Task<List<BusinessSubscription>> GetByBusinessId(Guid businessId);

        Task<BusinessSubscription?> GetCurrentSubscription(Guid businessId);

        Task<BusinessSubscription?> GetLatestByBusinessId(Guid businessId);
    }
}
