using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GestionTurnos.Application.Abstraction.Infrastructure.External_Interface;
using GestionTurnos.Application.Exceptions;
using Microsoft.Extensions.Configuration;

namespace GestionTurnos.Infrastructure.ExternalServices
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MercadoPagoService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private string GetAccessToken()
        {
            var token = _configuration["MercadoPago:AccessToken"];

            if (string.IsNullOrWhiteSpace(token) || token.StartsWith("PENDING", StringComparison.OrdinalIgnoreCase))
            {
                throw new ConflictException(
                    "MercadoPago no está configurado. Cargá la clave 'MercadoPago:AccessToken' en appsettings o en la variable de entorno 'MercadoPago__AccessToken'.");
            }

            return token;
        }

        public async Task<MercadoPagoPreferenceResult> CreatePreference(MercadoPagoPreferenceRequest request)
        {
            var client = _httpClientFactory.CreateClient("MercadoPago");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken());

            var backUrl = string.IsNullOrWhiteSpace(request.BackUrl)
                ? _configuration["MercadoPago:BackUrl"] ?? "http://localhost:5173"
                : request.BackUrl;

            if (!backUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                throw new ConflictException(
                    $"MercadoPago no está configurado. Cargá la clave 'MercadoPago:BackUrl' (URL del frontend) en appsettings o en la variable de entorno 'MercadoPago__BackUrl'. Valor actual: '{backUrl}'.");
            }

            var notificationUrl = string.IsNullOrWhiteSpace(request.NotificationUrl)
                ? _configuration["MercadoPago:NotificationUrl"]
                : request.NotificationUrl;

            var isLocalhost = backUrl.Contains("localhost", StringComparison.OrdinalIgnoreCase)
                              || backUrl.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase);

            var body = new Dictionary<string, object?>
            {
                ["items"] = new[]
                {
                    new Dictionary<string, object?>
                    {
                        ["id"] = request.ExternalReference,
                        ["title"] = request.Title,
                        ["quantity"] = 1,
                        ["unit_price"] = request.Amount,
                        ["currency_id"] = request.CurrencyId
                    }
                },
                ["external_reference"] = request.ExternalReference,
                ["back_urls"] = new Dictionary<string, string>
                {
                    ["success"] = backUrl,
                    ["failure"] = backUrl,
                    ["pending"] = backUrl
                },
                ["auto_return"] = isLocalhost ? null : "approved",
                ["notification_url"] = string.IsNullOrWhiteSpace(notificationUrl) ? null : notificationUrl
            };

            if (!string.IsNullOrWhiteSpace(request.PayerEmail))
            {
                body["payer"] = new Dictionary<string, object?> { ["email"] = request.PayerEmail };
            }

            var json = JsonSerializer.Serialize(body);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/checkout/preferences", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ConflictException($"MercadoPago rechazó la solicitud de checkout ({(int)response.StatusCode}): {responseContent}");
            }

            using var doc = JsonDocument.Parse(responseContent);
            var id = doc.RootElement.TryGetProperty("id", out var idEl) ? idEl.GetString() : null;
            var initPoint = doc.RootElement.TryGetProperty("init_point", out var ipEl) ? ipEl.GetString() : null;

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(initPoint))
            {
                throw new ConflictException("MercadoPago devolvió una respuesta sin preference id o init_point.");
            }

            return new MercadoPagoPreferenceResult { Id = id, InitPoint = initPoint };
        }

        public async Task<MercadoPagoPaymentInfo?> GetPayment(string paymentId)
        {
            var client = _httpClientFactory.CreateClient("MercadoPago");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken());

            var response = await client.GetAsync($"/v1/payments/{paymentId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ConflictException($"No se pudo consultar el pago en MercadoPago ({(int)response.StatusCode}): {responseContent}");
            }

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            return new MercadoPagoPaymentInfo
            {
                Id = root.TryGetProperty("id", out var idEl) ? idEl.ToString() ?? paymentId : paymentId,
                Status = root.TryGetProperty("status", out var statusEl) ? statusEl.GetString() ?? string.Empty : string.Empty,
                ExternalReference = root.TryGetProperty("external_reference", out var refEl) ? refEl.GetString() : null,
                TransactionAmount = root.TryGetProperty("transaction_amount", out var amountEl) ? amountEl.GetDecimal() : 0m
            };
        }

        // Busca el último pago asociado a una orden (external_reference). Se usa cuando
        // la back_url de MercadoPago no incluye payment_id (flujo sin auto_return, o
        // usuario que vuelve con el botón del navegador) para poder confirmar igual.
        public async Task<string?> SearchLatestPaymentIdByExternalReference(string externalReference)
        {
            if (string.IsNullOrWhiteSpace(externalReference))
            {
                return null;
            }

            var client = _httpClientFactory.CreateClient("MercadoPago");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken());

            var url = $"/v1/payments/search?external_reference={Uri.EscapeDataString(externalReference)}&sort=date_created&criteria=desc&limit=1";
            var response = await client.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ConflictException($"No se pudo buscar el pago en MercadoPago ({(int)response.StatusCode}): {responseContent}");
            }

            using var doc = JsonDocument.Parse(responseContent);
            if (doc.RootElement.TryGetProperty("results", out var results)
                && results.ValueKind == JsonValueKind.Array
                && results.GetArrayLength() > 0
                && results[0].TryGetProperty("id", out var firstId))
            {
                return firstId.ToString();
            }

            return null;
        }
    }
}
