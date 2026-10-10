using GestionTurnos.Application.Abstraction.Infrastructure;
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
    public class ScheduleController : ControllerBase
    {
       private readonly IScheduleService _scheduleService;

        public  ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [Authorize(Policy = Policies.AnyStaff)]
        [HttpGet("branch/{branchId}")]
        public async Task<ActionResult<List<ScheduleResponse>>> GetByBranch([FromRoute] Guid branchId)
        {
            return Ok(await _scheduleService.GetByBranch(branchId));
        }

        [Authorize(Policy = Policies.SysAdminOrAdmin)]
        [HttpPut("branch/{branchId}")]
        public async Task<ActionResult<List<ScheduleResponse>>> UpdateByBranch([FromRoute] Guid branchId, [FromBody] UpdateBranchSchedulesRequest request)
        {
            return Ok(await _scheduleService.UpdateBranchSchedules(branchId, request));
        }
    }
}
