using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class PaymentOrderRepository : BaseRepository<PaymentOrder>, IPaymentOrderRepository
    {
        public PaymentOrderRepository(FMCTurnosDbContext context) : base(context)
        {
        }

        public async Task<PaymentOrder?> GetByIdWithDetails(Guid id)
        {
            return await _dbSet
                .Include(po => po.Business)
                .Include(po => po.Plan)
                .FirstOrDefaultAsync(po => po.Id == id && !po.IsDeleted);
        }
    }
}
