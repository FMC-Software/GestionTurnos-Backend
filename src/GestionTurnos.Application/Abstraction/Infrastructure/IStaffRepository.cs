using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IStaffRepository : IBaseRepository<Staff>
    {
        public List<Staff> GetStaffOfBusiness(Guid id_business);

    }

}
