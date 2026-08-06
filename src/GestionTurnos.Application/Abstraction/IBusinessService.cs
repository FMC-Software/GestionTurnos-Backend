using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction
{
    public interface IBusinessService
    {
        Task<Business> Create(Business business);
        Task Delete();

        Task<List<BusinessDashboardResponse>> GetAllGlobal();

        Task<BusinessDashboardResponse> GetBusinessEcosystem();

        Task Update(BusinessUpdateRequest value);

        Task<Business> GetById(Guid id);

        Business initialBusiness(SignUpRequest request, TypeBusiness typeBusinessParsed);

        List<BusinessTypeResponse> GetBusinessTypes();
    }
}
