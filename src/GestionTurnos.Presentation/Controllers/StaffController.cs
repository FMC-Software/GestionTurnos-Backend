using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Request;
using GestionTurnos.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTurnos.Presentation.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }
        //[Authorize(Policy = "SysAdmin")]
        [HttpGet]
        public ActionResult<List<Staff>> GetAll()
        {

            return Ok(_staffService.GetAll());
        }
        //[Authorize(Policy = "SysAdmin")]
        [HttpGet("{id}")]
        public ActionResult<Staff> GetById(Guid id)
        {

            return Ok(_staffService.GetById(id));
        }
        //[Authorize(Policy = "Admin")]
        [HttpGet("Business/{businessId}")]
        public ActionResult<List<Staff>> GetStaffOfBusiness(Guid businessId)
        {

            return Ok(_staffService.GetStaffOfBusiness(businessId));
        }
        //[Authorize(Policy = "Admin")]
        [HttpPost]
        public ActionResult<Staff> CreateStaff([FromBody] StaffRequest Staff)
        {
           return Ok(_staffService.CreateStaff(Staff, Staff.BusinessId));

        }

        //[Authorize(Policy = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult DeleteStaff(Guid id)
        {
            _staffService.DeleteStaff(id);
            return NoContent();
        }

        //[Authorize(Policy = "Admin")]
        [HttpPut("{id}")]
        public ActionResult<Staff> UpdateStaff([FromBody] StaffRequest Staff, Guid id)
        {
            var updatedUser = _staffService.UpdateStaff(Staff, id);
            return Ok(updatedUser);
        }
    }
}
