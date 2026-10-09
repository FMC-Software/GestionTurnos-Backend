using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Mapper
{
    public static class PaymentOrderMapper
    {
        public static PaymentOrderResponse ToPaymentOrderResponse(this PaymentOrder order)
        {
            return new PaymentOrderResponse
            {
                Id = order.Id,
                BusinessId = order.BusinessId,
                PlanId = order.PlanId,
                Kind = order.Kind.ToString(),
                Amount = order.Amount,
                Currency = order.Currency,
                Status = order.Status.ToString(),
                MercadoPagoPaymentId = order.MercadoPagoPaymentId,
                UpdateDateTime = order.UpdateDateTime
            };
        }
    }
}
