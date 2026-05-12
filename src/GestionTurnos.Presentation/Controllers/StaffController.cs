using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Request;
using GestionTurnos.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GestionTurnos.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public ActionResult<List<Staff>> GetAll()
        {

            return Ok(_staffService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Staff> GetById(Guid id)
        {

            return _staffService.GetById(id);
        }

        [HttpPost]
        public ActionResult<Staff> CreateStaffWhitBusiness([FromBody] BusinessRequest user)
        {
            var newUser = _staffService.CreateStaffWhitBusiness(user);
            return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);
        }
    }
}
