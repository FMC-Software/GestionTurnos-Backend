using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class BusinessSubscriptionRepository : BaseRepository<BusinessSubscription>, IBusinessSubscriptionRepository
    {
        public BusinessSubscriptionRepository(FMCTurnosDbContext context) : base(context)
        {
        }

        public async Task<List<BusinessSubscription>> GetActiveSubscriptionsAsync()
        {
            return await _dbSet
                .Include(bs => bs.Business)
                .Where(bs => bs.Status == Status.Active)
                .ToListAsync();
        }

        public async Task UpdateAsync(BusinessSubscription entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<BusinessSubscription>> GetAllWithDetails()
        {
            return await _dbSet
                .Include(bs => bs.Business)
                .Include(bs => bs.Plan)
                .Where(bs => !bs.IsDeleted)
                .ToListAsync();
        }

        public async Task<BusinessSubscription?> GetByIdWithDetails(Guid id)
        {
            return await _dbSet
                .Include(bs => bs.Business)
                .Include(bs => bs.Plan)
                .FirstOrDefaultAsync(bs => bs.Id == id);
        }

        public async Task<List<BusinessSubscription>> GetByBusinessId(Guid businessId)
        {
            return await _dbSet
                .Include(bs => bs.Business)
                .Include(bs => bs.Plan)
                .Where(bs => bs.BusinessId == businessId && !bs.IsDeleted)
                .ToListAsync();
        }

        public async Task<BusinessSubscription?> GetCurrentSubscription(Guid businessId)
        {
            return await _dbSet
                .Include(bs => bs.Business)
                .Include(bs => bs.Plan)
                .FirstOrDefaultAsync(bs =>
                    bs.BusinessId == businessId &&
                    bs.Status == Status.Active &&
                    !bs.IsDeleted);

        }

        public async Task<BusinessSubscription?> GetLatestByBusinessId(Guid businessId)
        {
            return await _dbSet
                .Include(bs => bs.Business)
                .Include(bs => bs.Plan)
                .Where( bs =>
                    bs.BusinessId == businessId &&
                    !bs.IsDeleted)
                .OrderByDescending(bs => bs.EndDate)
                .FirstOrDefaultAsync();
        }
    }
}
