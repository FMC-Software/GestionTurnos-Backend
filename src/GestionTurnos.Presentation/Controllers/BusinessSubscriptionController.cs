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
        public ActionResult<List<BusinessSubscriptionResponse>> GetAll()
        {

                var subscriptions = _subscriptionService.GetAll();
                return Ok(subscriptions);

        }

        //Retorna una suscripcion a traves del Id del Business
        [Authorize(Policy = Policies.SysAdmin)]
        [HttpGet("{id}")]
        public ActionResult<BusinessSubscriptionResponse> GetById([FromRoute] Guid id)
        {

                var subscription = _subscriptionService.GetById(id);
                return Ok(subscription);
 
        }




        //Crea una suscripcion, asignando un plan a un business
        [Authorize(Policy = Policies.SysAdmin)]
        [HttpPost]
        public ActionResult<BusinessSubscriptionResponse> Create([FromBody] BusinessSubscriptionRequest request)
        {

                var newSubscription = _subscriptionService.Create(request);
                return CreatedAtAction(nameof(GetById), new { id = newSubscription.Id }, newSubscription);

        }

        // Cambiar el estado de una suscripcion
        [Authorize(Policy = Policies.SysAdmin)]
        [HttpPut("{id}/status")]
        public ActionResult<BusinessSubscriptionResponse> UpdateStatus(
            [FromRoute] Guid id,
            [FromBody] UpdateSubscriptionStatusRequest request)
        {
            
            
                var updated = _subscriptionService.UpdateStatus(id, request.Status);
                return Ok(updated);

        }

        //Desactiva una suscripcion 
        [Authorize(Policy = Policies.SysAdmin)]
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] Guid id)
        {

                _subscriptionService.Delete(id);
                return NoContent();

        }

        [Authorize(Policy = Policies.Admin)]
        [HttpGet("my")]
        public ActionResult<BusinessSubscriptionResponse> GetMy()
        {

                var businessId = _tenantProvider.GetBusinessId()
                    ?? throw new ConflictException("No se encontro el negocio en el token");

                var subscription = _subscriptionService.GetCurrentSubscription(businessId);

                return Ok(subscription);

        }

        //Retorna el historial del negocio
        [Authorize(Policy = Policies.Admin)]
        [HttpGet("my/history")]
        public ActionResult<List<BusinessSubscriptionResponse>> GetMyHistory()
        {

                var businessId = _tenantProvider.GetBusinessId()
                    ?? throw new ConflictException("No se encontró el negocio en el token.");

                var subscriptions = _subscriptionService.GetByBusinessId(businessId);
                return Ok(subscriptions);

        }

        [Authorize(Policy = Policies.Admin)]
        [HttpPut("my/renew")]
        public IActionResult Renew()
        {
  
                var businessId = _tenantProvider.GetBusinessId()
                    ?? throw new ConflictException("No se encontro el negocio en el token");

                _subscriptionService.RenewSubscription(businessId);

                return NoContent();

        
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpPut("my/change-plan/{planId}")]
        public IActionResult ChangePlan([FromRoute] Guid planId)
        {

                var businessId = _tenantProvider.GetBusinessId()
                    ?? throw new ConflictException("No se encontro el negocio en el token");

                _subscriptionService.ChangePlan(businessId, planId);

                return NoContent();
         

        }

    }
}