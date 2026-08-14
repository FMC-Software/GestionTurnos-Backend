using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IBranchRepository : IBaseRepository<Branch>
    {
        
        Task<List<Branch>> GetByBusinessId(Guid businessId);

        Task<Branch?> GetInfoBranch( Guid branchId);
    }
}
