using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction
{
    public interface IBusinessService
    {
        Business Create(Business business);
        void Delete();

        List<BusinessDashboardResponse> GetAllGlobal();

        BusinessDashboardResponse GetBusinessEcosystem();

        void Update(BusinessUpdateRequest value);

        Business GetById(Guid id);
    }
}