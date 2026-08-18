using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace GestionTurnos.Infrastructure.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public async Task JoinGroup(string businessId)
        {
            var claimValue = Context.User?.FindFirst("BusinessId")?.Value;

            if (Context.User?.Identity?.IsAuthenticated != true || string.IsNullOrEmpty(claimValue))
            {
                throw new HubException("Autenticación requerida para unirse a un grupo de notificaciones.");
            }

            if (!Guid.TryParse(businessId, out var requestedBusinessId) ||
                !Guid.TryParse(claimValue, out var tokenBusinessId) ||
                requestedBusinessId != tokenBusinessId)
            {
                throw new HubException("No se puede unir al grupo de otro negocio.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"business-{tokenBusinessId}");
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Conexión SignalR establecida: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Conexión SignalR finalizada: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}