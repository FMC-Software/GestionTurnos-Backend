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

 
}