using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTurnos.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessSubscriptionController : ControllerBase
    {
        private readonly IBusinessSubscriptionService _subscriptionService;
        private readonly ITenantProvider _tenantProvider;

        public BusinessSubscriptionController(
            IBusinessSubscriptionService subscriptionService,
            ITenantProvider tenantProvider)
        {
            _subscriptionService = subscriptionService;
            _tenantProvider = tenantProvider;
        }

        // Retorna todas las suscripciones para control del SysAdmin
        [Authorize(Policy = Policies.SysAdmin)]
        [HttpGet]
        public async Task<ActionResult<List<BusinessSubscriptionResponse>>> GetAll()
        {

                var subscriptions = await _subscriptionService.GetAll();
                return Ok(subscriptions);

        }

        //Retorna una suscripcion a traves del Id del Business
        [Authorize(Policy = Policies.SysAdmin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<BusinessSubscriptionResponse>> GetById([FromRoute] Guid id)
        {

                var subscription = await _subscriptionService.GetById(id);
                return Ok(subscription);

        }




        //Crea una suscripcion, asignando un plan a un business
        [Authorize(Policy = Policies.SysAdmin)]
        [HttpPost]
        public async Task<ActionResult<BusinessSubscriptionResponse>> Create([FromBody] BusinessSubscriptionRequest request)
        {

                var newSubscription = await _subscriptionService.Create(request);
                return CreatedAtAction(nameof(GetById), new { id = newSubscription.Id }, newSubscription);

        }

        // Cambiar el estado de una suscripcion
        [Authorize(Policy = Policies.SysAdmin)]
        [HttpPut("{id}/status")]
        public async Task<ActionResult<BusinessSubscriptionResponse>> UpdateStatus(
            [FromRoute] Guid id,
            [FromBody] UpdateSubscriptionStatusRequest request)
        {


                var updated = await _subscriptionService.UpdateStatus(id, request.Status);
                return Ok(updated);

        }

        //Desactiva una suscripcion
        [Authorize(Policy = Policies.SysAdmin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {

                await _subscriptionService.Delete(id);
                return NoContent();

        }

        [Authorize(Policy = Policies.Admin)]
        [HttpGet("my")]
        public async Task<ActionResult<BusinessSubscriptionResponse>> GetMy()
        {

                var businessId = _tenantProvider.GetBusinessId()
                    ?? throw new ConflictException("No se encontro el negocio en el token");

                var subscription = await _subscriptionService.GetCurrentSubscription(businessId);

                return Ok(subscription);

        }

        //Retorna el historial del negocio
        [Authorize(Policy = Policies.Admin)]
        [HttpGet("my/history")]
        public async Task<ActionResult<List<BusinessSubscriptionResponse>>> GetMyHistory()
        {

                var businessId = _tenantProvider.GetBusinessId()
                    ?? throw new ConflictException("No se encontró el negocio en el token.");

                var subscriptions = await _subscriptionService.GetByBusinessId(businessId);
                return Ok(subscriptions);

        }

        [Authorize(Policy = Policies.Admin)]
        [HttpPut("my/renew")]
        public async Task<IActionResult> Renew()
        {

                var businessId = _tenantProvider.GetBusinessId()
                    ?? throw new ConflictException("No se encontro el negocio en el token");

                await _subscriptionService.RenewSubscription(businessId);

                return NoContent();


        }

        [Authorize(Policy = Policies.Admin)]
        [HttpPut("my/change-plan/{planId}")]
        public async Task<IActionResult> ChangePlan([FromRoute] Guid planId)
        {

                var businessId = _tenantProvider.GetBusinessId()
                    ?? throw new ConflictException("No se encontro el negocio en el token");

                await _subscriptionService.ChangePlan(businessId, planId);

                return NoContent();


        }

    }
}
