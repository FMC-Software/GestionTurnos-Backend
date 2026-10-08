namespace GestionTurnos.Domain.Entities
{
    public enum PaymentOrderKind
    {
        ChangePlan,
        Renew
    }

    public enum PaymentOrderStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled,
        Expired
    }

    public class PaymentOrder : BaseEntity
    {
        public Guid BusinessId { get; set; }
        public Business Business { get; set; } = null!;

        public Guid PlanId { get; set; }
        public Plan Plan { get; set; } = null!;

        public PaymentOrderKind Kind { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "ARS";
        public PaymentOrderStatus Status { get; set; } = PaymentOrderStatus.Pending;

        public string? MercadoPagoPreferenceId { get; set; }
        public string? MercadoPagoPaymentId { get; set; }
    }
}
