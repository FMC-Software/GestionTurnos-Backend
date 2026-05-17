using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IBusinessRepository : IBaseRepository<Business>
    {
        List<Business> GetAllByBusiness(Guid id_Business);
    }
}