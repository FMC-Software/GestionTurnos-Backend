using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    public class SysAdminRepository : BaseRepository<SysAdminUser>, ISysAdminRepository
    {
        
        public SysAdminRepository(FMCTurnosDbContext context, ITenantProvider tenantProvider) : base(context)
        {
           
        }

        public async Task<SysAdminUser?> GetByEmail(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);
        }
    }
}
