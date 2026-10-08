namespace GestionTurnos.Application.Abstraction.Infrastructure.External_Interface
{
    public interface IMercadoPagoService
    {
        Task<MercadoPagoPreferenceResult> CreatePreference(MercadoPagoPreferenceRequest request);

        Task<MercadoPagoPaymentInfo?> GetPayment(string paymentId);

        Task<string?> SearchLatestPaymentIdByExternalReference(string externalReference);
    }

    public class MercadoPagoPreferenceRequest
    {
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string CurrencyId { get; set; } = "ARS";
        public string ExternalReference { get; set; } = string.Empty;
        public string BackUrl { get; set; } = string.Empty;
        public string? NotificationUrl { get; set; }
        public string? PayerEmail { get; set; }
    }

    public class MercadoPagoPreferenceResult
    {
        public string Id { get; set; } = string.Empty;
        public string InitPoint { get; set; } = string.Empty;
    }

    public class MercadoPagoPaymentInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ExternalReference { get; set; }
        public decimal TransactionAmount { get; set; }
    }
}
