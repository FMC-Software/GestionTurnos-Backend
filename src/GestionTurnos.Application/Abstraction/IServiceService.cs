using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;

namespace GestionTurnos.Application.Abstraction
{
    public interface IServiceService
    {
        Task<List<ServiceBusinessResponse>> GetServicesOfCurrentBusiness();
        Task<ServiceBusinessResponse> GetById(Guid id);
        Task<ServiceBusinessResponse> CreateService(ServiceRequest request);
        Task<ServiceBusinessResponse> UpdateService(ServiceRequest request, Guid id);
        Task DeleteService(Guid id);
    }
}
