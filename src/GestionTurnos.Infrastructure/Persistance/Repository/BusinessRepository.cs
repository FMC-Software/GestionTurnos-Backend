using GestionTurnos.Application.Abstraction.Infrastructure;
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

    public Business? GetBusinessWithEcosystem()
    {
        var currentTenantId = _tenantProvider.GetBusinessId();

        return _dbSet.Include(b => b.Branches.Where(br => br.IsDeleted != true))
                      .Include(b => b.Services.Where(s => s.IsDeleted != true))
                      .Include(b => b.Clients.Where(c => c.IsDeleted != true))
                      .FirstOrDefault(b => b.Id == currentTenantId && !b.IsDeleted);
    }

    public override List<Business> GetAllGlobal()
    {
        return _dbSet.Include(b => b.Branches) 
                       .Include(b => b.Services)
                       .Include(b => b.Clients)
                       .ToList();
    }
}