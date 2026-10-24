using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Helpers;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IScheduleService _scheduleService;
        private readonly ITenantProvider _tenantProvider;
        private readonly IBusinessService _businessService;
        private readonly IBusinessSubscriptionService _businessSubscriptionService;
        private readonly IStaffRepository _staffRepository;

        public BranchService(IBranchRepository branchRepository, ITenantProvider tenantProvider, IScheduleService scheduleService, IBusinessService businessService, IBusinessSubscriptionService businessSubscriptionService, IStaffRepository staffRepository)
        {
            _branchRepository = branchRepository;
            _tenantProvider = tenantProvider;
            _scheduleService = scheduleService;
            _businessService = businessService;
            _businessSubscriptionService = businessSubscriptionService;
            _staffRepository = staffRepository;
        }

        public async Task<List<BranchResponse>> GetBranchesOfCurrentBusiness()
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var branches = await _branchRepository.GetByBusinessId(businessId);
            return branches
                .Where(b => !b.IsDeleted)
                .Select(b => b.ToBranchResponse())
                .ToList();
        }

        // Publico (AllowAnonymous): solo sucursales habilitadas para mostrar.
        public async Task<List<BranchResponse>> GetBranchesByBusinessId(Guid businessId)
        {
            var branches = await _branchRepository.GetByBusinessId(businessId);
            return branches.Where(b => b.IsActive).Select(b => b.ToBranchResponse()).ToList();
        }

        public async Task<BranchResponse> GetById(Guid id)
        {
            var branch = await _branchRepository.GetById(id)
                ?? throw new ConflictException("Sucursal no encontrada.");

            return branch.ToBranchResponse();
        }

        public async Task<BranchResponse> CreateBranch(CreateBranchRequest request)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var businessBranches = await _branchRepository.GetByBusinessId(businessId);

            bool branchExist = businessBranches
                .Any( b =>
                    string.Equals(
                        b.Name.Trim(),
                        request.Name.Trim(),
                        StringComparison.OrdinalIgnoreCase));

            bool branchAdrressDuplicated = businessBranches
                .Any(b =>
                    string.Equals(
                        b.Address.Trim(),
                        request.Address.Trim(),
                        StringComparison.OrdinalIgnoreCase));


            if (branchExist)
            {
                throw new ConflictException("Ya existe una sucursal con ese nombre");
            }

            if (branchAdrressDuplicated)
            {
                throw new ConflictException("Ya tienes una sucursal con esa direccion.");
            }

            var plan = await _businessSubscriptionService.GetActivePlanForBusiness(businessId);
            PlanLimitGuard.EnsureWithinLimit(businessBranches.Count(b => b.IsActive), plan.MaxBranchesAllowed, "sucursales");

            var newBranch = request.ToBranch(businessId);

            await _branchRepository.Add(newBranch);

            for (int i = 0; i < 7; i++)
            {
                await _scheduleService.CreateSchedule(new ScheduleRequest
                {
                    BranchId = newBranch.Id,
                    Day = (DayOfWeek)i,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    SlotDurationMinutes = 30,
                    IsActive = (i == 0 || i == 5)
                });
            }

            return newBranch.ToBranchResponse();
        }

        public async Task<BranchResponse> UpdateBranch(CreateBranchRequest request, Guid id)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontro la empresa");

            var existingBranch = await _branchRepository.GetById(id)
                ?? throw new ConflictException("Sucursal no encontrada");

            if(existingBranch.BusinessId != businessId)
            {
                throw new ConflictException("La sucursal no pertenece a su negocio");
            }

            var businessBranches = await _branchRepository.GetByBusinessId(businessId);
            bool branchExist = businessBranches
                .Any(b =>
                    b.Id != id &&
                    string.Equals(
                        b.Name.Trim(),
                        request.Name.Trim(),
                        StringComparison.OrdinalIgnoreCase));

            if (branchExist)
            {
                throw new ConflictException("Ya existe una sucursal con ese nombre");
            }

            if (request.IsActive == true && !existingBranch.IsActive)
                await EnsureCanActivate(businessId);

            if (request.IsActive == false && existingBranch.IsActive
                && (await _staffRepository.GetByBusinessIdGlobal(businessId)).Any(s => s.Rol == Rol.Admin && s.BranchId == id))
                throw new ConflictException("No se puede desactivar la sucursal del Admin.");

            existingBranch.UpdateFromRequest(request);

            await _branchRepository.Update(existingBranch);

            return existingBranch.ToBranchResponse();

        }

        public async Task DeleteBranch(Guid id)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var branch = await _branchRepository.GetById(id)
                ?? throw new ConflictException("Sucursal no encontrada.");

            if (branch.BusinessId != businessId)
            {
                throw new ConflictException(
                    "La sucursal no pertenece a su negocio.");
            }

            await _branchRepository.Delete(id);
        }

        public async Task<Branch> CreateInitialBranch(SignUpRequest request, Business newBusiness)
        {
            // Corregido: Usamos el Mapper para crear la entidad
            var newBranch = new Branch
            {
                Id = Guid.NewGuid(),
                Address = request.Address,
                Phone = request.BranchPhone,
                BusinessId = newBusiness.Id,
                Name = "Surcursal 1",
                City = request.City,
                Business = newBusiness
            };

            await _branchRepository.Add(newBranch);

            // Lógica de los 7 días
            for (int i = 0; i < 7; i++)
            {
                await _scheduleService.CreateSchedule(new ScheduleRequest
                {
                    BranchId = newBranch.Id,
                    Day = (DayOfWeek)i,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    SlotDurationMinutes = 30,
                    IsActive = (i == 0 || i == 5)
                });
            }
            return newBranch;
        }


        public async Task<InfoBranchResponse> GetInfoBranch(Guid idBranch)
        {

            var branch = await _branchRepository.GetInfoBranch(idBranch)
                ?? throw new ConflictException("Sucursal no encontrada.");

            return branch.ToInfoBranchResponse();
        }

        // Para el Admin del negocio: reactiva una sucursal propia respetando el limite del plan.
        public async Task<BranchResponse> ActivateBranch(Guid id)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró la empresa.");

            var branch = await _branchRepository.GetById(id)
                ?? throw new ConflictException("Sucursal no encontrada.");

            if (branch.BusinessId != businessId)
                throw new ConflictException("La sucursal no pertenece a su negocio.");

            if (branch.IsActive)
                return branch.ToBranchResponse();

            await EnsureCanActivate(businessId);

            branch.IsActive = true;
            branch.UpdateDateTime = DateTime.UtcNow;
            await _branchRepository.Update(branch);
            return branch.ToBranchResponse();
        }

        // Reactivar cuenta contra el cupo del plan igual que crear: no se puede superar MaxBranchesAllowed.
        private async Task EnsureCanActivate(Guid businessId)
        {
            var plan = await _businessSubscriptionService.GetActivePlanForBusiness(businessId);
            var activeCount = (await _branchRepository.GetByBusinessId(businessId)).Count(b => b.IsActive);
            PlanLimitGuard.EnsureWithinLimit(activeCount, plan.MaxBranchesAllowed, "sucursales");
        }

        // Para SysAdmin: habilita/deshabilita cualquier sucursal de cualquier
        // negocio, sin el chequeo de pertenencia que usa UpdateBranch (Admin).
        public async Task<BranchResponse> SetActiveStatus(Guid branchId, bool isActive)
        {
            var branch = await _branchRepository.GetById(branchId)
                ?? throw new ConflictException("Sucursal no encontrada.");

            branch.IsActive = isActive;
            branch.UpdateDateTime = DateTime.UtcNow;

            await _branchRepository.Update(branch);

            return branch.ToBranchResponse();
        }
    }
}

