using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;

namespace GestionTurnos.Application.Abstraction
{
    public interface IAppointmentService
    {
        Task<List<AppointmentResponse>> GetAppointmentsOfCurrentBusiness();
        Task<List<AppointmentResponse>> GetAppointmentsOfMyBranch();
        Task<List<AppointmentResponse>> GetMyAppointments();
        Task<List<AppointmentResponse>> GetAppointmentsByBranch(Guid branchId);
        Task<List<GlobalAppointmentResponse>> GetAllGlobal();
        Task<AppointmentResponse> CreateAppointment(AppointmentRequest request);
        Task<AppointmentResponse> GetById(Guid id);
        Task<AppointmentResponse> UpdateAppointment(Guid id, AppointmentRequest request);
        Task<AppointmentResponse> UpdateStatus(Guid id, GestionTurnos.Domain.Entities.AppointmentStatus newStatus);
        Task DeleteAppointment(Guid id);

        Task<List<AvailableSlotResponse>> GetAvailableSlots(Guid branchId, Guid staffId, Guid serviceId, DateTime date);

        Task<List<AppointmentResponse>> GetAppointmentsByBranchAndDate(DateTime day, Guid? branchId = null);

        Task<BranchAgendaResponse> GetBranchAgenda(Guid branchId, DateTime date);

    }
}
