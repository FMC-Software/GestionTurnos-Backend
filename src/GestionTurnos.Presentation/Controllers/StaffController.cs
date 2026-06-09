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

        [Authorize(Policy = Policies.Admin)]
        [HttpGet("Business/Staffs")]
        public ActionResult<List<StaffsResponse>> GetStaffOfBusiness()
        {
                var staffs = _staffService.GetStaffOfCurrentBusiness();
                return Ok(staffs);

        }
        [Authorize(Policy = Policies.SysAdminOrAdmin)]
        [HttpPost]
        public ActionResult<StaffsResponse> CreateStaff([FromBody] StaffRequest user)
        {
            return Ok(_staffService.CreateStaff(user));
        }

        [Authorize(Policy = Policies.SysAdminOrAdmin)]
        [HttpDelete("{id}")]
        public ActionResult DeleteStaff([FromRoute] Guid id)
        {
            _staffService.DeleteStaff(id);
            return NoContent();
        }

         [Authorize(Policy = Policies.SysAdminOrAdmin)]
        [HttpPut("{id}")]
        public ActionResult<StaffsResponse> UpdateStaff([FromBody] StaffRequest user, [FromRoute] Guid id)
        {
            var updatedUser = _staffService.UpdateStaff(user, id);
            return Ok(updatedUser);
        }
    }
}
