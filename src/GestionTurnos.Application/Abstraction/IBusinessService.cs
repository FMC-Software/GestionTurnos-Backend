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

        // Igual que Update, pero para que SysAdmin edite cualquier negocio (no solo el propio).
        Task UpdateByAdmin(Guid businessId, BusinessUpdateRequest value);

        Task<Business> GetById(Guid id);

        Task<Business> initialBusiness(SignUpRequest request, TypeBusiness typeBusinessParsed);

        List<BusinessTypeResponse> GetBusinessTypes();

        Task<List<BusinessSummaryResponse>> GetBusinessesByType(TypeBusiness type);

        Task<BusinessPublicResponse> GetPublicByUrl(string url);

        Task<BusinessPublicEcosystemResponse> GetPublicEcosystemByUrl(string url);
    }
}
