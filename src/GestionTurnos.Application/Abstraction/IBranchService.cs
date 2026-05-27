using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction
{
    public interface IBranchService
    {
        List<BranchResponse> GetBranchesOfCurrentBusiness();
        BranchResponse GetById(Guid id);
        BranchResponse CreateBranch(CreateBranchRequest request);
        BranchResponse UpdateBranch(CreateBranchRequest request, Guid id);
        void DeleteBranch(Guid id);
    }
}
