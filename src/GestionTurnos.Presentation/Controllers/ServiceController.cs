using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTurnos.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateService(ServiceRequest request)
        {
            await _serviceService.CreateService(request);
            return Ok();
        }

        [HttpGet]

        public async Task<IActionResult> GetServicesOfCurrentBusiness()
        {
           var Services = await _serviceService.GetServicesOfCurrentBusiness();
            return  Ok(Services);
        }

        [AllowAnonymous]
        [HttpGet("/api/services/business/{businessId}")]
        public async Task<ActionResult<List<ServiceResponse>>> GetByBusinessId([FromRoute] Guid businessId)
        {
            return Ok(await _serviceService.GetServicesByBusinessId(businessId));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(ServiceRequest request, [FromRoute] Guid id)
        {
            await _serviceService.UpdateService(request, id);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService([FromRoute] Guid id)
        {
            await _serviceService.DeleteService(id);
            return NoContent();
        }
    }
}
