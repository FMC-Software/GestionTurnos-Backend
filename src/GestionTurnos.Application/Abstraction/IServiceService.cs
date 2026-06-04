using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;

namespace GestionTurnos.Application.Abstraction
{
    public interface IServiceService
    {
        List<ServiceBusinessResponse> GetServicesOfCurrentBusiness();
        ServiceBusinessResponse GetById(Guid id);
        ServiceBusinessResponse CreateService(ServiceRequest request);
        ServiceBusinessResponse UpdateService(ServiceRequest request, Guid id);
        void DeleteService(Guid id);
    }
}
