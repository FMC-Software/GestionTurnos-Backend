using GestionTurnos.Application.Response;

namespace GestionTurnos.Application.Abstraction
{
    public interface IAppointmentRealtimeNotifier
    {
        Task NotifyAppointmentCreatedAsync(AppointmentNotificationPayload payload);
    }
}