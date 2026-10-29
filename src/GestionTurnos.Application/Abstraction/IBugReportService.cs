using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;

namespace GestionTurnos.Application.Abstraction
{
    public interface IBugReportService
    {
        Task<BugReportResponse> Create(BugReportCreateRequest request);
        Task<List<BugReportResponse>> GetAll();
        Task<BugReportResponse> UpdateStatus(Guid id, BugReportStatusUpdateRequest request);
    }
}
