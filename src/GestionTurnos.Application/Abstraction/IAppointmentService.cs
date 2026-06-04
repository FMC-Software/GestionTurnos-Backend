using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;

namespace GestionTurnos.Application.Abstraction
{
    public interface IAppointmentService
    {
        List<AppointmentResponse> GetAppointmentsOfCurrentBusiness();
        List<AppointmentResponse> GetAppointmentsOfMyBranch();
        List<AppointmentResponse> GetMyAppointments();
        List<AppointmentResponse> GetAppointmentsByBranch(Guid branchId);
        List<GlobalAppointmentResponse> GetAllGlobal();
        AppointmentResponse CreateAppointment(AppointmentRequest request);
        AppointmentResponse GetById(Guid id);
        AppointmentResponse UpdateAppointment(Guid id, AppointmentRequest request);
        AppointmentResponse UpdateStatus(Guid id, GestionTurnos.Domain.Entities.AppointmentStatus newStatus);
        void DeleteAppointment(Guid id);
        
    }
}