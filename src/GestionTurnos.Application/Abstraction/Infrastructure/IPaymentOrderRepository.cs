using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IPaymentOrderRepository : IBaseRepository<PaymentOrder>
    {
        Task<PaymentOrder?> GetByIdWithDetails(Guid id);
    }
}
