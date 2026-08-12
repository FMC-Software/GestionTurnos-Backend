using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction
{
    public interface IBranchService
    {
        Task<List<BranchResponse>> GetBranchesOfCurrentBusiness();
        Task<List<BranchResponse>> GetBranchesByBusinessId(Guid businessId);
        Task<BranchResponse> GetById(Guid id);
        Task<BranchResponse> CreateBranch(CreateBranchRequest request);
        Task<BranchResponse> UpdateBranch(CreateBranchRequest request, Guid id);
        Task DeleteBranch(Guid id);

        public Task<Branch> CreateInitialBranch(SignUpRequest request, Business newBusiness);

        public Task<InfoBranchResponse> GetInfoBranch(Guid idBranch);


    }
}
