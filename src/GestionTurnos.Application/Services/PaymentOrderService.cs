using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Abstraction.Infrastructure.External_Interface;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class PaymentOrderService : IPaymentOrderService
    {
        private readonly IPaymentOrderRepository _paymentOrderRepository;
        private readonly IBusinessSubscriptionRepository _subscriptionRepository;
        private readonly IBusinessSubscriptionService _subscriptionService;
        private readonly IPlanService _planService;
        private readonly IMercadoPagoService _mercadoPagoService;

        public PaymentOrderService(
            IPaymentOrderRepository paymentOrderRepository,
            IBusinessSubscriptionRepository subscriptionRepository,
            IBusinessSubscriptionService subscriptionService,
            IPlanService planService,
            IMercadoPagoService mercadoPagoService)
        {
            _paymentOrderRepository = paymentOrderRepository;
            _subscriptionRepository = subscriptionRepository;
            _subscriptionService = subscriptionService;
            _planService = planService;
            _mercadoPagoService = mercadoPagoService;
        }

        public async Task<CheckoutResponse> CreateChangePlanCheckout(Guid businessId, Guid planId)
        {
            var currentSubscription = await _subscriptionRepository
                .GetCurrentSubscription(businessId)
                ?? await _subscriptionRepository.GetLatestByBusinessId(businessId)
                ?? throw new NotFoundException("El negocio no posee suscripciones");

            var newPlan = await _planService.GetActivePlan(planId);

            if (currentSubscription.PlanId == newPlan.Id)
            {
                throw new ConflictException("El negocio ya posee este plan");
            }

            return await CreateOrderOrApply(businessId, newPlan, PaymentOrderKind.ChangePlan);
        }

        public async Task<CheckoutResponse> CreateRenewCheckout(Guid businessId)
        {
            var subscription = await _subscriptionRepository
                .GetCurrentSubscription(businessId)
                ?? await _subscriptionRepository.GetLatestByBusinessId(businessId)
                ?? throw new ConflictException("El negocio no posee suscripciones");

            if (subscription.Status == Status.Cancelled)
            {
                throw new ConflictException("No se puede renovar una suscripcion cancelada");
            }

            if (subscription.Plan == null)
            {
                throw new ConflictException("La suscripción no tiene un plan asociado.");
            }

            return await CreateOrderOrApply(businessId, subscription.Plan, PaymentOrderKind.Renew);
        }

        private async Task<CheckoutResponse> CreateOrderOrApply(Guid businessId, Plan plan, PaymentOrderKind kind)
        {
            if (plan.Price <= 0)
            {
                if (kind == PaymentOrderKind.ChangePlan)
                {
                    await _subscriptionService.ChangePlan(businessId, plan.Id);
                }
                else
                {
                    await _subscriptionService.RenewSubscription(businessId);
                }

                return new CheckoutResponse
                {
                    OrderId = null,
                    Status = "noPaymentRequired"
                };
            }

            var order = new PaymentOrder
            {
                Id = Guid.NewGuid(),
                BusinessId = businessId,
                PlanId = plan.Id,
                Kind = kind,
                Amount = plan.Price,
                Currency = "ARS",
                Status = PaymentOrderStatus.Pending
            };

            await _paymentOrderRepository.Add(order);

            try
            {
                var preference = await _mercadoPagoService.CreatePreference(new MercadoPagoPreferenceRequest
                {
                    Title = kind == PaymentOrderKind.ChangePlan
                        ? $"Cambio de plan a {plan.Name}"
                        : $"Renovación del plan {plan.Name}",
                    Amount = plan.Price,
                    CurrencyId = "ARS",
                    ExternalReference = order.Id.ToString()
                });

                order.MercadoPagoPreferenceId = preference.Id;
                await _paymentOrderRepository.Update(order);

                return new CheckoutResponse
                {
                    OrderId = order.Id,
                    Status = "pending",
                    InitPoint = preference.InitPoint
                };
            }
            catch
            {
                order.Status = PaymentOrderStatus.Cancelled;
                await _paymentOrderRepository.Update(order);
                throw;
            }
        }

        public async Task<PaymentOrderResponse> ProcessPaymentNotification(string paymentId)
        {
            var payment = await _mercadoPagoService.GetPayment(paymentId)
                ?? throw new ConflictException("Pago no encontrado en MercadoPago");

            if (string.IsNullOrWhiteSpace(payment.ExternalReference)
                || !Guid.TryParse(payment.ExternalReference, out var orderId))
            {
                throw new ConflictException("El pago no tiene un external_reference válido.");
            }

            var order = await _paymentOrderRepository.GetByIdWithDetails(orderId)
                ?? throw new NotFoundException("Orden de pago no encontrada");

            if (order.Status != PaymentOrderStatus.Pending)
            {
                return order.ToPaymentOrderResponse();
            }

            order.MercadoPagoPaymentId = payment.Id;

            if (payment.TransactionAmount < order.Amount)
            {
                throw new ConflictException("El monto pagado no coincide con el monto de la orden.");
            }

            var mappedStatus = MapStatus(payment.Status);

            if (mappedStatus == PaymentOrderStatus.Pending)
            {
                await _paymentOrderRepository.Update(order);
                return order.ToPaymentOrderResponse();
            }

            if (mappedStatus == PaymentOrderStatus.Approved)
            {
                try
                {
                    if (order.Kind == PaymentOrderKind.ChangePlan)
                    {
                        await _subscriptionService.ChangePlan(order.BusinessId, order.PlanId);
                    }
                    else
                    {
                        await _subscriptionService.RenewSubscription(order.BusinessId);
                    }
                }
                catch (ConflictException)
                {
                    // El pago se acreditó pero la acción ya no aplica (ej: el plan ya fue cambiado).
                    // Igualmente se marca la orden como aprobada.
                }
            }

            order.Status = mappedStatus;
            await _paymentOrderRepository.Update(order);

            return order.ToPaymentOrderResponse();
        }

        public async Task<CheckoutResponse> GetStatus(Guid orderId, Guid businessId, string? paymentId)
        {
            var order = await _paymentOrderRepository.GetByIdWithDetails(orderId)
                ?? throw new NotFoundException("Orden de pago no encontrada");

            if (order.BusinessId != businessId)
            {
                throw new NotFoundException("Orden de pago no encontrada");
            }

            if (order.Status == PaymentOrderStatus.Pending)
            {
                var effectivePaymentId = paymentId;

                // Si la back_url no trajo payment_id, buscamos el último pago en MercadoPago
                // por la referencia de la orden (mismo criterio que usa el webhook).
                if (string.IsNullOrWhiteSpace(effectivePaymentId))
                {
                    try
                    {
                        effectivePaymentId = await _mercadoPagoService
                            .SearchLatestPaymentIdByExternalReference(orderId.ToString());
                    }
                    catch (Exception)
                    {
                        effectivePaymentId = null;
                    }
                }

                if (!string.IsNullOrWhiteSpace(effectivePaymentId))
                {
                    try
                    {
                        await ProcessPaymentNotification(effectivePaymentId);
                        order = await _paymentOrderRepository.GetByIdWithDetails(orderId)
                            ?? throw new NotFoundException("Orden de pago no encontrada");
                    }
                    catch (Exception)
                    {
                        // Si MercadoPago no responde (o no está configurado en dev), devolvemos
                        // el estado actual de la orden y el frontend sigue haciendo polling.
                    }
                }
            }

            return new CheckoutResponse
            {
                OrderId = order.Id,
                Status = order.Status.ToString().ToLowerInvariant(),
                InitPoint = null
            };
        }

        private static PaymentOrderStatus MapStatus(string mercadoPagoStatus)
        {
            return mercadoPagoStatus switch
            {
                "approved" => PaymentOrderStatus.Approved,
                "rejected" => PaymentOrderStatus.Rejected,
                "cancelled" => PaymentOrderStatus.Cancelled,
                "expired" => PaymentOrderStatus.Expired,
                _ => PaymentOrderStatus.Pending
            };
        }
    }
}
