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
        private readonly IScheduleService _scheduleService;
        private readonly ITenantProvider _tenantProvider;
        private readonly IBusinessService _businessService;

        public BranchService(IBranchRepository branchRepository, ITenantProvider tenantProvider, IScheduleService scheduleService, IBusinessService businessService)
        {
            _branchRepository = branchRepository;
            _tenantProvider = tenantProvider;
            _scheduleService = scheduleService;
            _businessService = businessService;
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

            var newBranch = request.ToBranch(businessId);
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

        public Branch CreateInitialBranch(SignUpRequest request, Business newBusiness)
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

            _branchRepository.Add(newBranch);

            // Lógica de los 7 días
            for (int i = 0; i < 7; i++)
            {
                _scheduleService.CreateSchedule(new ScheduleRequest
                {
                    BranchId = newBranch.Id,
                    Day = (DayOfWeek)i,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    
                    IsDeleted = (i != 0 && i != 6)
                });
            }
            return newBranch;
        }


        public InfoBranchResponse GetInfoBranch(Guid idBusiness, Guid idBranch)
        {
            var businessExisting = _businessService.GetById(idBusiness) ?? throw new ConflictException("Empresa no encontrada.");
            var branch = _branchRepository.GetInfoBranch(idBusiness, idBranch)
                ?? throw new ConflictException("Sucursal no encontrada.");

            return branch.ToInfoBranchResponse();
        }
    }
}

