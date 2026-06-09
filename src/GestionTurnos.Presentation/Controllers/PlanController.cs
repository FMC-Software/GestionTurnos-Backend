using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTurnos.Presentation.Controllers
{
    [Authorize(Policy = Policies.Admin)] // Cambie a Admin para testeo, volver a poner SysAdmin
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
            try
            {
                var plans = _planService.GetAll();
                return Ok(plans);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error inesperado.");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<PlanResponse> GetById([FromRoute] Guid id)
        {
            try
            {
                var plan = _planService.GetById(id);
                return Ok(plan);
            }
            catch (ConflictException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error inesperado.");
            }
        }

        [HttpPost]
        public ActionResult<PlanResponse> Create([FromBody] PlanRequest request)
        {
            try
            {
                var newPlan = _planService.Create(request);
                return CreatedAtAction(nameof(GetById), new { id = newPlan.Id }, newPlan);
            }
            catch (ConflictException ex)
            {
                return StatusCode(409, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error inesperado.");
            }
        }

        [HttpPut("{id}")]
        public ActionResult<PlanResponse> Update([FromBody] PlanRequest request, [FromRoute] Guid id)
        {
            try
            {
                var updatedPlan = _planService.Update(request, id);
                return Ok(updatedPlan);
            }
            catch (ConflictException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error inesperado.");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] Guid id)
        {
            try
            {
                _planService.Delete(id);
                return NoContent();
            }
            catch (ConflictException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error inesperado.");
            }
        }
    }
}
