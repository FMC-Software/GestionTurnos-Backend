using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTurnos.Presentation.Controllers
{
    [Authorize(Policy = Policies.SysAdmin)] 
    [Route("api/[controller]")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly IPlanService _planService;

        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }

        [HttpGet]
        public ActionResult<List<PlanResponse>> GetAll()
        {
            var plans = _planService.GetAll();
            return Ok(plans);
        }

        [HttpGet("{id}")]
        public ActionResult<PlanResponse> GetById([FromRoute] Guid id)
        {
            var plan = _planService.GetById(id);
            return Ok(plan);
        }

        [HttpPost]
        public ActionResult<PlanResponse> Create([FromBody] PlanRequest request)
        {
            var newPlan = _planService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = newPlan.Id }, newPlan);
        }

        [HttpPut("{id}")]
        public ActionResult<PlanResponse> Update([FromBody] PlanRequest request, [FromRoute] Guid id)
        {
            var updatedPlan = _planService.Update(request, id);
            return Ok(updatedPlan);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] Guid id)
        {
            _planService.Delete(id);
            return NoContent();
        }
    }
}
