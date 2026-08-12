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
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [AllowAnonymous]
        [HttpGet("branch/{branchId}")]
        public async Task<ActionResult<List<StaffSummaryResponse>>> GetStaffByBranch([FromRoute] Guid branchId)
        {
            return Ok(await _staffService.GetStaffByBranchId(branchId));
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpGet("Business/Staffs")]
        public async Task<ActionResult<List<StaffsResponse>>> GetStaffOfBusiness()
        {
                var staffs = await _staffService.GetStaffOfCurrentBusiness();
                return Ok(staffs);

        }
        [Authorize(Policy = Policies.SysAdminOrAdmin)]
        [HttpPost]
        public async Task<ActionResult<StaffsResponse>> CreateStaff([FromBody] StaffRequest user)
        {
            return Ok(await _staffService.CreateStaff(user));
        }

        [Authorize(Policy = Policies.SysAdminOrAdmin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStaff([FromRoute] Guid id)
        {
            await _staffService.DeleteStaff(id);
            return NoContent();
        }

         [Authorize(Policy = Policies.SysAdminOrAdmin)]
        [HttpPut("{id}")]
        public async Task<ActionResult<StaffsResponse>> UpdateStaff([FromBody] StaffRequest user, [FromRoute] Guid id)
        {
            var updatedUser = await _staffService.UpdateStaff(user, id);
            return Ok(updatedUser);
        }
    }
}
