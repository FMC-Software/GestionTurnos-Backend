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
        public ActionResult<List<Business>> GetAllGlobal()
        {
            return Ok(_businessService.GetAllGlobal());

        }



        [Authorize(Policy = Policies.Admin)]
        [HttpGet("MyBusiness")] 
        public ActionResult<BusinessDashboardResponse> GetMyBusinessWithEcosystem()
        {

                var businessEcosystem = _businessService.GetBusinessEcosystem();
                return Ok(businessEcosystem);
 
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpPut("/MyBusiness/Update")]
        public ActionResult UpdateMyBusiness([FromBody] BusinessUpdateRequest request)
        {
            _businessService.Update(request);
            return NoContent();
        }
        
        [Authorize(Policy = Policies.Admin)]
        [HttpDelete("/MyBusiness/Delete")]
        public ActionResult<bool> Delete()
        {

                _businessService.Delete();
                return Ok(true);

        }
    }
}