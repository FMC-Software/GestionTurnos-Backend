using GestionTurnos.Application.Abstraction;
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
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public ActionResult<List<BranchResponse>> GetAll()
        {
            try
            {
                var branches = _branchService.GetBranchesOfCurrentBusiness();
                return Ok(branches);
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

        [HttpGet("{id}")]
        public ActionResult<BranchResponse> GetById([FromRoute] Guid id)
        {
            try
            {
                var branch = _branchService.GetById(id);
                return Ok(branch);
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

        [Authorize(Policy = Policies.Admin)]
        [HttpPost]
        public ActionResult<BranchResponse> Create([FromBody] CreateBranchRequest request)
        {
            try
            {
                var newBranch = _branchService.CreateBranch(request);
                return CreatedAtAction(nameof(GetById), new { id = newBranch.Id }, newBranch);
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

        [Authorize(Policy = Policies.Admin)]
        [HttpPut("{id}")]
        public ActionResult<BranchResponse> Update([FromBody] CreateBranchRequest request, [FromRoute] Guid id)
        {
            try
            {
                var updatedBranch = _branchService.UpdateBranch(request, id);
                return Ok(updatedBranch);
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

        [Authorize(Policy = Policies.Admin)]
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] Guid id)
        {
            try
            {
                _branchService.DeleteBranch(id);
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
