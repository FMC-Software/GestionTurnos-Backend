using GestionTurnos.Application.Response;
using System.Threading.Tasks;

namespace GestionTurnos.Application.Abstraction
{
    public interface IDashboardService
    {
        Task<DashboardSummaryResponse> GetDashboard();
    }
}