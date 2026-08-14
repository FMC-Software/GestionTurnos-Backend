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

        [Authorize(Policy = Policies.Admin)]
        [HttpGet]
        public async Task<ActionResult<List<BranchResponse>>> GetAll()
        {

                var branches = await _branchService.GetBranchesOfCurrentBusiness();
                return Ok(branches);

        }
        [Authorize(Policy = Policies.Admin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<BranchResponse>> GetById([FromRoute] Guid id)
        {

                var branch = await _branchService.GetById(id);
                return Ok(branch);

        }
        [AllowAnonymous]
        [HttpGet("/api/branches/business/{businessId}")]
        public async Task<ActionResult<List<BranchResponse>>> GetByBusinessId([FromRoute] Guid businessId)
        {
            return Ok(await _branchService.GetBranchesByBusinessId(businessId));
        }

        [AllowAnonymous]
        [HttpGet("InfoBranch/{idBranch}")]
        public async Task<ActionResult<BranchResponse>> GetInfoBranch( [FromRoute] Guid idBranch)
        {

                var branch = await _branchService.GetInfoBranch(idBranch);
                return Ok(branch);

        }

        [Authorize(Policy = Policies.Admin)]
        [HttpPost]
        public async Task<ActionResult<BranchResponse>> Create([FromBody] CreateBranchRequest request)
        {

                var newBranch = await _branchService.CreateBranch(request);
                return CreatedAtAction(nameof(GetById), new { id = newBranch.Id }, newBranch);

        }

        [Authorize(Policy = Policies.Admin)]
        [HttpPut("{id}")]
        public async Task<ActionResult<BranchResponse>> Update([FromBody] CreateBranchRequest request, [FromRoute] Guid id)
        {

                var updatedBranch = await _branchService.UpdateBranch(request, id);
                return Ok(updatedBranch);
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {

                await _branchService.DeleteBranch(id);
                return NoContent();

        }
    }
}
