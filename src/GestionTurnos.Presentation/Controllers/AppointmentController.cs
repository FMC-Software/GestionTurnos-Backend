using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Request;
using GestionTurnos.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTurnos.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        //[HttpGet("global")]
        //public ActionResult GetAllGlobal()
        //{
        //    var appointments = _appointmentService.GetAllGlobal();
        //    return Ok(appointments);
        //}

        [Authorize(Policy = Policies.Admin)]
        [HttpGet]
        public ActionResult Get() {
            var appointments = _appointmentService.GetAppointmentsOfCurrentBusiness();
            return Ok(appointments);
        }

        [HttpGet("my-branch")]// solo para Recepcionistas, el service no deja que el profesional los obtenga.
        public ActionResult GetMyBranchAppointments() {
            var appointments = _appointmentService.GetAppointmentsOfMyBranch();
            return Ok(appointments);
        }

        [Authorize(Policy = Policies.Profesional)]
        [HttpGet("my-appointments")]
        public ActionResult GetMyAppointments() {
            var appointments = _appointmentService.GetMyAppointments();
            return Ok(appointments);
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpGet("branch/{branchId}")]
        public ActionResult GetByBranch(Guid branchId) {
            var appointments = _appointmentService.GetAppointmentsByBranch(branchId);
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public ActionResult GetById(Guid id)
        {
            var appointment = _appointmentService.GetById(id);
            return Ok(appointment);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Post([FromBody] AppointmentRequest request)
        {
            var appointment = _appointmentService.CreateAppointment(request);
            return Ok(appointment);
        }

        [HttpPut("{id}")]
        public ActionResult Update(Guid id, [FromBody] AppointmentRequest request)
        {
            var appointment = _appointmentService.UpdateAppointment(id, request);
            return Ok(appointment);
        }

        [HttpPatch("{id}/status")]
        public ActionResult UpdateStatus(Guid id, [FromBody] UpdateAppointmentStatusRequest request)
        {
            var appointment = _appointmentService.UpdateStatus(id, request.Status);
            return Ok(appointment);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            _appointmentService.DeleteAppointment(id);
            return NoContent();
        }
    }
}
