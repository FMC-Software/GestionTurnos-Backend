using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;
        private readonly ITenantProvider _tenantProvider;

        public BranchService(IBranchRepository branchRepository, ITenantProvider tenantProvider)
        {
            _branchRepository = branchRepository;
            _tenantProvider = tenantProvider;
        }

        public List<BranchResponse> GetBranchesOfCurrentBusiness()
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            return _branchRepository.GetByBusinessId(businessId)
                .Where(b => !b.IsDeleted)
                .Select(b => b.ToBranchResponse())
                .ToList();
        }

        public BranchResponse GetById(Guid id)
        {
            var branch = _branchRepository.GetById(id)
                ?? throw new ConflictException("Sucursal no encontrada.");

            return branch.ToBranchResponse();
        }

        public BranchResponse CreateBranch(CreateBranchRequest request)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var newBranch = request.ToBranch();
            newBranch.BusinessId = businessId;

            _branchRepository.Add(newBranch);

            return newBranch.ToBranchResponse();
        }

        public BranchResponse UpdateBranch(CreateBranchRequest request, Guid id)
        {
            var existingBranch = _branchRepository.GetById(id)
                ?? throw new ConflictException("Sucursal no encontrada.");

            existingBranch.UpdateFromRequest(request);

            _branchRepository.Update(existingBranch);

            return existingBranch.ToBranchResponse();
        }

        public void DeleteBranch(Guid id)
        {
            var branch = _branchRepository.GetById(id)
                ?? throw new ConflictException("Sucursal no encontrada.");

            _branchRepository.Delete(id);
        }
    }
}

