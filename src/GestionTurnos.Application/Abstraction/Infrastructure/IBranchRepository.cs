using GestionTurnos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IBranchRepository : IBaseRepository<Branch>
    {
        //bool CreateBranch(Branch branch);
        List<Branch> GetByBusinessId(Guid businessId);

        Branch? GetInfoBranch(Guid businessId, Guid branchId);
    }
}
