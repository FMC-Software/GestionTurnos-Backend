using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IStaffRepository : IBaseRepository<Staff>
    {
        public List<Staff> GetAll();

        public Staff GetByEmail(string email);

        public Staff GetByEmailGlobal(string email);

    }

}
