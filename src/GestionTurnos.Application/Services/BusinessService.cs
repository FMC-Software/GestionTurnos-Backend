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

        public Business Create(Business business)
        {
            return _businessRepository.Add(business);
        }

        public void Delete()
        {
            var BusinesId = _tenantProvider.GetBusinessId() ?? throw new ConflictException("No se encontró la empresa.");

            _businessRepository.Delete(BusinesId);
        }

        public List<BusinessDashboardResponse> GetAllGlobal()
        {
            return _businessRepository.GetAllGlobal()
                .Select(b => b.ToResponse())
                .ToList();
        }

        public Business GetById(Guid id)
        {
            var business = _businessRepository.GetById(id) ?? throw new ConflictException("Empresa no encontrada.");
            return business;
        }
        public BusinessDashboardResponse GetBusinessEcosystem()
        {
            var business = _businessRepository.GetById(_tenantProvider.GetBusinessId() ?? Guid.Empty)
                ?? throw new ConflictException("No se encontró la configuración de su empresa.");
          
            return business.ToResponse();
        }

        public void Update(BusinessUpdateRequest value)
        {
            var BusinesId = _tenantProvider.GetBusinessId();

            var existingBusiness = _businessRepository.GetById(BusinesId ?? Guid.Empty)
                ?? throw new KeyNotFoundException("Empresa no encontrada");

            existingBusiness.Name = value.Name;


            _businessRepository.Update(existingBusiness);
        }

        public Business initialBusiness(SignUpRequest request, TypeBusiness typeBusinessParsed)
        {
            var newBusiness = new Business
            {
                Id = Guid.NewGuid(),
                Name = $"{request.Name} - {request.BusinessCategory}",
                Url = $"http://www.{request.Name.Replace(" ", "")}.FCMTurniFy.com",
                TypeBusiness = typeBusinessParsed
            };
            return newBusiness;
        }
    }
}