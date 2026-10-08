using GestionTurnos.Application.Response;

namespace GestionTurnos.Application.Abstraction
{
    public interface IPaymentOrderService
    {
        Task<CheckoutResponse> CreateChangePlanCheckout(Guid businessId, Guid planId);

        Task<CheckoutResponse> CreateRenewCheckout(Guid businessId);

        Task<PaymentOrderResponse> ProcessPaymentNotification(string paymentId);

        Task<CheckoutResponse> GetStatus(Guid orderId, Guid businessId, string? paymentId);
    }
}
