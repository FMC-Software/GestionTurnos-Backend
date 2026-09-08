using GestionTurnos.Application.Response;

namespace GestionTurnos.Application.Abstraction
{
    public interface ISysAdminDashboardService
    {
        Task<SysAdminDashboardResponse> GetDashboard();
    }
}
