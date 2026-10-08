namespace GestionTurnos.Application.Response
{
    public class CheckoutResponse
    {
        public Guid? OrderId { get; set; }

        // "pending" -> hay que pagar en MercadoPago, "noPaymentRequired" -> se aplico directo (plan gratis)
        public string Status { get; set; } = string.Empty;

        public string? InitPoint { get; set; }
    }

    public class PaymentOrderResponse
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public Guid PlanId { get; set; }
        public string Kind { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? MercadoPagoPaymentId { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}
