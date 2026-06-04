using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
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
    public class SysAdminController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public SysAdminController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [Authorize(Policy = "SysAdmin")]
        [HttpGet]
        public ActionResult<List<GlobalStaffResponse>> GetAll()
        {

            return Ok(_staffService.GetStaffOfCurrentBusiness());
        }
        
        [Authorize(Policy = "SysAdmin")]
        [HttpGet("{id}")]
        public ActionResult<GlobalStaffResponse> GetById(Guid id)
        {

            return Ok(_staffService.GetById(id));
        }

        [Authorize(Policy = "SysAdmin")]
        [HttpPut("{id}")]
        public ActionResult<Staff> UpdateStaff([FromBody] StaffRequest Staff, [FromRoute] Guid id)
        {
            var updatedUser = _staffService.UpdateStaff(Staff, id);
            return Ok(updatedUser);
        }
    }
}
