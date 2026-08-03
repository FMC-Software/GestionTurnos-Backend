using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction.Infrastructure
{
    public interface ISysAdminRepository : IBaseRepository<SysAdminUser>
    {
        Task<SysAdminUser?> GetByEmail(string email);
    }
}
