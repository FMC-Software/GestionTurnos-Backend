using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistance;
using GestionTurnos.Infrastructure.Persistance.Repository;
using GestionTurnos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class BusinessRepository : BaseRepository<Business>, IBusinessRepository
{
    private readonly ITenantProvider _tenantProvider;

    public BusinessRepository(FMCTurnosDbContext context, ITenantProvider tenantProvider) : base(context)
    {
        _tenantProvider = tenantProvider;
    }

    public async Task<List<Business>> GetByType(TypeBusiness type)
    {
        return await _dbSet
            .Where(b => b.TypeBusiness == type && !b.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<BusinessCardResponse>> GetBusinessCardsAsync()
    {
        return await _dbSet
            .Where(b => !b.IsDeleted)
            .Select(b => new BusinessCardResponse
            {
                Id = b.Id,
                Name = b.Name,
                UrlLogo = b.UrlLogo,
                TypeBusiness = b.TypeBusiness,
                Status = b.IsActive,
                BranchCount = b.Branches.Count(x => !x.IsDeleted),
                ClientCount = b.Clients.Count(x => !x.IsDeleted),
                StaffCount = _context.Staffs.Count(s => s.BusinessId == b.Id && !s.IsDeleted)
            })
            .ToListAsync();
    }

    public async Task<BusinessStatsResult?> GetBusinessStatsAsync(Guid businessId)
    {
        return await _dbSet
            .Where(b => b.Id == businessId && !b.IsDeleted)
            .Select(b => new BusinessStatsResult
            {
                BranchCount = b.Branches.Count(x => !x.IsDeleted),
                ClientCount = b.Clients.Count(x => !x.IsDeleted),
                StaffCount = _context.Staffs.Count(s => s.BusinessId == b.Id && !s.IsDeleted),
                AppointmentCount = _context.Appointments.Count(a => a.Staff.BusinessId == b.Id && !a.IsDeleted)
            })
            .FirstOrDefaultAsync();
    }
}