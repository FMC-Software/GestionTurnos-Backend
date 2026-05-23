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
        [HttpGet("Business/{businessId}")]
        public ActionResult<List<StaffsResponse>> GetStaffOfBusiness([FromRoute] Guid businessId) // MICAEL PREGUNTALE AL PROFE SI USAR TENANT
        {
            /*var ClaimBusinessId = HttpContext.User.Claims.Where(c => c.Type == "BusinessId");
            if(ClaimBusinessId.ToString() != businessId.ToString())
            {
                return Forbid();
            }*/
            return Ok(_staffService.GetStaffOfBusiness(businessId));
        }
        [Authorize(Policy = Policies.Admin)]
        [HttpPost]
        public ActionResult<StaffsResponse> CreateStaff([FromBody] StaffRequest Staff)
        {
           return Ok(_staffService.CreateStaff(Staff, Staff.BusinessId));

        }

        [Authorize(Policy = Policies.Admin)]
        [HttpDelete("{id}")]
        public ActionResult DeleteStaff([FromRoute] Guid id)
        {
            _staffService.DeleteStaff(id);
            return NoContent();
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpPut("{id}")]
        public ActionResult<Staff> UpdateStaff([FromBody] StaffRequest Staff, [FromRoute] Guid id)
        {
            var updatedUser = _staffService.UpdateStaff(Staff, id);
            return Ok(updatedUser);
        }
    }
}
