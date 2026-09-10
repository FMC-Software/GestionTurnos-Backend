using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IBusinessRepository : IBaseRepository<Business>
    {
        Task<List<Business>> GetByType(TypeBusiness type);

        Task<List<BusinessCardResponse>> GetBusinessCardsAsync();

        Task<BusinessStatsResult?> GetBusinessStatsAsync(Guid businessId);
    }
}