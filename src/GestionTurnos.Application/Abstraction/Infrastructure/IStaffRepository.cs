using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface IStaffRepository : IBaseRepository<Staff>
    {
        public Task<List<Staff>> GetAll();

        public Task<Staff?> GetByEmail(string email);

        public Task<Staff?> GetByEmailGlobal(string email);

        public Task<List<Staff>> GetByBranchId(Guid branchId);

        public Task<Staff?> GetAdminOfCurrentBusiness();

    }

}
