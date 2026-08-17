using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Response;
using GestionTurnos.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GestionTurnos.Infrastructure.ExternalServices
{
    public class AppointmentRealtimeNotifier : IAppointmentRealtimeNotifier
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public AppointmentRealtimeNotifier(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyAppointmentCreatedAsync(AppointmentNotificationPayload payload)
        {
            try
            {
                await _hubContext.Clients
                    .Group($"business-{payload.BusinessId}")
                    .SendAsync("NewAppointment", payload);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando notificación de turno por SignalR: {ex.Message}");
            }
        }
    }
}