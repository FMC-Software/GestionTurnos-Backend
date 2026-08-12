using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Exceptions;
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
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessService _businessService;

        public BusinessController(IBusinessService businessService)
        {
            _businessService = businessService;
        }

        [Authorize(Policy = "SysAdmin")]
        [HttpGet("global")]
        public async Task<ActionResult<List<BusinessDashboardResponse>>> GetAllGlobal()
        {
            return Ok(await _businessService.GetAllGlobal());

        }

        [AllowAnonymous]
        [HttpGet("types")]
        public ActionResult<List<BusinessTypeResponse>> GetBusinessTypes()
        {
            return Ok(_businessService.GetBusinessTypes());
        }

        [AllowAnonymous]
        [HttpGet("type/{typeBusiness}")]
        public async Task<ActionResult<List<BusinessSummaryResponse>>> GetBusinessesByType([FromRoute] TypeBusiness typeBusiness)
        {
            return Ok(await _businessService.GetBusinessesByType(typeBusiness));
        }



        [Authorize(Policy = Policies.Admin)]
        [HttpGet("MyBusiness")]
        public async Task<ActionResult<BusinessDashboardResponse>> GetMyBusinessWithEcosystem()
        {

                var businessEcosystem = await _businessService.GetBusinessEcosystem();
                return Ok(businessEcosystem);

        }

        [Authorize(Policy = Policies.Admin)]
        [HttpPut("MyBusiness/Update")]
        public async Task<ActionResult> UpdateMyBusiness([FromBody] BusinessUpdateRequest request)
        {
            await _businessService.Update(request);
            return NoContent();
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpDelete("MyBusiness/Delete")]
        public async Task<ActionResult<bool>> Delete()
        {

                await _businessService.Delete();
                return Ok(true);

        }
    }
}
