using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Response;
using GestionTurnos.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GestionTurnos.Presentation.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentOrderService _paymentOrderService;
        private readonly ITenantProvider _tenantProvider;

        public PaymentController(
            IPaymentOrderService paymentOrderService,
            ITenantProvider tenantProvider)
        {
            _paymentOrderService = paymentOrderService;
            _tenantProvider = tenantProvider;
        }

        // Webhook de MercadoPago. Viene sin autenticación y sin token nuestro,
        // por eso se valida todo contra la API de MercadoPago (nunca se confía en el payload).
        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            string? type = Request.Query["type"].FirstOrDefault() ?? Request.Query["topic"].FirstOrDefault();
            string? paymentId = Request.Query["data.id"].FirstOrDefault() ?? Request.Query["id"].FirstOrDefault();

            if (Request.Body.CanSeek)
            {
                Request.Body.Position = 0;
            }

            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();

            if (!string.IsNullOrWhiteSpace(rawBody))
            {
                try
                {
                    using var doc = JsonDocument.Parse(rawBody);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("type", out var typeEl))
                    {
                        type = typeEl.GetString() ?? type;
                    }

                    if (root.TryGetProperty("data", out var dataEl)
                        && dataEl.TryGetProperty("id", out var idEl))
                    {
                        paymentId = idEl.GetString() ?? paymentId;
                    }
                    else if (root.TryGetProperty("id", out var rootIdEl))
                    {
                        paymentId = rootIdEl.GetString() ?? paymentId;
                    }
                }
                catch (JsonException)
                {
                    // Body que no es JSON: lo ignoramos y usamos solo la query string.
                }
            }

            // Solo nos interesan las notificaciones de pagos.
            if (!string.IsNullOrWhiteSpace(type)
                && !string.Equals(type, "payment", StringComparison.OrdinalIgnoreCase))
            {
                return Ok();
            }

            if (string.IsNullOrWhiteSpace(paymentId))
            {
                return Ok();
            }

            await _paymentOrderService.ProcessPaymentNotification(paymentId);

            // MercadoPago espera 200 siempre; si falla, reintenta más adelante.
            return Ok();
        }

        // Estado de la orden para que el frontend haga polling (necesario en dev local
        // donde MercadoPago no puede alcanzar el webhook). Si viene un paymentId
        // (query de la back_url), se confirma contra la API de MercadoPago en el momento.
        [Authorize(Policy = Policies.Admin)]
        [HttpGet("{orderId:guid}/status")]
        public async Task<ActionResult<CheckoutResponse>> GetStatus(
            [FromRoute] Guid orderId,
            [FromQuery] string? paymentId)
        {
            var businessId = _tenantProvider.GetBusinessId()
                ?? throw new ConflictException("No se encontró el negocio en el token.");

            var status = await _paymentOrderService.GetStatus(orderId, businessId, paymentId);

            return Ok(status);
        }
    }
}
