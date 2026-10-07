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
    public class BusinessService : IBusinessService
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IStaffService _staffService;
        private readonly ITenantProvider _tenantProvider;
        private readonly IBranchRepository _branchRepository;
        private readonly IScheduleRepository _scheduleRepository;

        public BusinessService(IBusinessRepository businessRepository, ITenantProvider tenantProvider, IStaffService staffService, IBranchRepository branchRepository, IScheduleRepository scheduleRepository)
        {

            _businessRepository = businessRepository;
            _tenantProvider = tenantProvider;
            _staffService = staffService;
            _branchRepository = branchRepository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<Business> Create(Business business)
        {
            return await _businessRepository.Add(business);
        }

        public async Task Delete()
        {
            var BusinesId = _tenantProvider.GetBusinessId() ?? throw new ConflictException("No se encontró la empresa.");

            await _businessRepository.Delete(BusinesId);
        }

        public async Task<List<BusinessDashboardResponse>> GetAllGlobal()
        {
            var businesses = await _businessRepository.GetAllGlobal();
            return businesses
                .Select(b => b.ToResponse())
                .ToList();
        }

        public async Task<Business> GetById(Guid id)
        {
            var business = await _businessRepository.GetById(id) ?? throw new ConflictException("Empresa no encontrada.");
            return business;
        }
        public async Task<BusinessDashboardResponse> GetBusinessEcosystem()
        {
            var business = await _businessRepository.GetById(_tenantProvider.GetBusinessId() ?? Guid.Empty)
                ?? throw new ConflictException("No se encontró la configuración de su empresa.");

            return business.ToResponse();
        }

        public async Task Update(BusinessUpdateRequest request)
        {
            var BusinesId = _tenantProvider.GetBusinessId();

            var existingBusiness = await _businessRepository.GetById(BusinesId ?? Guid.Empty)
                ?? throw new KeyNotFoundException("Empresa no encontrada");

            if (!string.IsNullOrWhiteSpace(request.Url))
            {
                var normalizedUrl = SlugHelper.Slugify(request.Url);

                var businessWithSameUrl = await _businessRepository.GetByUrl(normalizedUrl);
                if (businessWithSameUrl != null && businessWithSameUrl.Id != existingBusiness.Id)
                    throw new ConflictException("Ya existe una empresa con esa URL.");

                request.Url = normalizedUrl;
            }

            existingBusiness.ToUpdateBusiness(request);


            await _businessRepository.Update(existingBusiness);
        }

        public async Task<Business> initialBusiness(SignUpRequest request, TypeBusiness typeBusinessParsed)
        {
            var baseSlug = SlugHelper.Slugify(request.Name);
            if (string.IsNullOrEmpty(baseSlug))
                baseSlug = "empresa";

            var url = baseSlug;
            var suffix = 1;
            while (await _businessRepository.ExistsByUrl(url))
            {
                suffix++;
                url = $"{baseSlug}-{suffix}";
            }

            var newBusiness = new Business
            {
                Id = Guid.NewGuid(),
                Name = $"{request.Name} - {typeBusinessParsed}",
                Url = url,
                TypeBusiness = typeBusinessParsed
            };
            return newBusiness;
        }

        public async Task<BusinessPublicResponse> GetPublicByUrl(string url)
        {
            var business = await ResolveEnabledBusinessByUrl(url);
            return business.ToPublicResponse();
        }

        public async Task<BusinessPublicEcosystemResponse> GetPublicEcosystemByUrl(string url)
        {
            var business = await ResolveEnabledBusinessByUrl(url);

            var branches = await _branchRepository.GetByBusinessId(business.Id);
            var branchResponses = new List<BranchPublicResponse>();

            foreach (var branch in branches)
            {
                var schedules = await _scheduleRepository.GetByBranchId(branch.Id);
                var staff = await _staffService.GetStaffByBranchId(branch.Id);

                branchResponses.Add(new BranchPublicResponse
                {
                    Id = branch.Id,
                    Name = branch.Name,
                    Address = branch.Address,
                    Phone = branch.Phone,
                    City = branch.City,
                    Schedules = schedules.Select(s => s.ToResponseSchedule()).ToList(),
                    Staff = staff
                });
            }

            return new BusinessPublicEcosystemResponse
            {
                Id = business.Id,
                Name = business.Name,
                Url = business.Url,
                LogoUrl = business.UrlLogo ?? string.Empty,
                Category = business.TypeBusiness,
                Branches = branchResponses
            };
        }

        private async Task<Business> ResolveEnabledBusinessByUrl(string url)
        {
            var normalizedUrl = SlugHelper.Slugify(url);

            var business = await _businessRepository.GetByUrl(normalizedUrl);
            if (business == null || business.IsActive != StatusBusiness.Habilitado)
                throw new NotFoundException("Empresa no encontrada.");

            return business;
        }

        public List<BusinessTypeResponse> GetBusinessTypes()
        {
            return Enum.GetValues<TypeBusiness>()
                .Select(t => new BusinessTypeResponse
                {
                    Id = (int)t,
                    Name = t.ToString()
                })
                .ToList();
        }

        public async Task<List<BusinessSummaryResponse>> GetBusinessesByType(TypeBusiness type)
        {
            var businesses = await _businessRepository.GetByType(type);
            return businesses.Select(b => b.ToSummaryResponse()).ToList();
        }
    }
}
