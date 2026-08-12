using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
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

        public BusinessService(IBusinessRepository businessRepository, ITenantProvider tenantProvider, IStaffService staffService)
        {

            _businessRepository = businessRepository;
            _tenantProvider = tenantProvider;
            _staffService = staffService;
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

            existingBusiness.ToUpdateBusiness(request);


            await _businessRepository.Update(existingBusiness);
        }

        public Business initialBusiness(SignUpRequest request, TypeBusiness typeBusinessParsed)
        {
            var newBusiness = new Business
            {
                Id = Guid.NewGuid(),
                Name = $"{request.Name} - {typeBusinessParsed}",
                Url = $"http://www.{request.Name.Replace(" ", "")}.FCMTurniFy.com",
                TypeBusiness = typeBusinessParsed
            };
            return newBusiness;
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
