using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class BranchLimitEnforcer : IBranchLimitEnforcer
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IStaffRepository _staffRepository;

        public BranchLimitEnforcer(IBranchRepository branchRepository, IStaffRepository staffRepository)
        {
            _branchRepository = branchRepository;
            _staffRepository = staffRepository;
        }

        public async Task ApplyBranchLimitAsync(Guid businessId, Plan plan)
        {
            if (plan.MaxBranchesAllowed < 0)
                return;

            var activeBranches = (await _branchRepository.GetByBusinessId(businessId))
                .Where(b => b.IsActive)
                .ToList();

            if (activeBranches.Count <= plan.MaxBranchesAllowed)
                return;

            // La sucursal del Admin siempre se conserva (y cuenta para el limite); el resto se ordena por antiguedad.
            var adminBranchIds = (await _staffRepository.GetByBusinessIdGlobal(businessId))
                .Where(s => s.IsActive && s.Rol == Rol.Admin)
                .Select(s => s.BranchId)
                .ToHashSet();

            var toKeep = activeBranches
                .OrderByDescending(b => adminBranchIds.Contains(b.Id))
                .ThenBy(b => b.CreatedDateTime)
                .Take(Math.Max(plan.MaxBranchesAllowed, activeBranches.Count(b => adminBranchIds.Contains(b.Id))))
                .Select(b => b.Id)
                .ToHashSet();

            foreach (var branch in activeBranches.Where(b => !toKeep.Contains(b.Id)))
            {
                branch.IsActive = false;
                branch.UpdateDateTime = DateTime.UtcNow;
                await _branchRepository.Update(branch);
            }
        }
    }
}
