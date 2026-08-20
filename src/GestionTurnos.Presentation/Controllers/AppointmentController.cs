using GestionTurnos.Application.Abstraction;
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
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [Authorize(Policy = Policies.SysAdmin)]
        [HttpGet("global")]
        public async Task<ActionResult> GetAllGlobal()
        {
            var appointments = await _appointmentService.GetAllGlobal();
            return Ok(appointments);
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpGet]
        public async Task<ActionResult> Get() {
            var appointments = await _appointmentService.GetAppointmentsOfCurrentBusiness();
            return Ok(appointments);
        }

        [Authorize(Policy = Policies.Recepcionista)]
        [HttpGet("my-branch")]
        public async Task<ActionResult> GetMyBranchAppointments() {
            var appointments = await _appointmentService.GetAppointmentsOfMyBranch();
            return Ok(appointments);
        }

        [Authorize(Policy = Policies.Profesional)]
        [HttpGet("my-appointments")]
        public async Task<ActionResult> GetMyAppointments() {
            var appointments = await _appointmentService.GetMyAppointments();
            return Ok(appointments);
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpGet("branch/{branchId}")]
        public async Task<ActionResult> GetByBranch(Guid branchId) {
            var appointments = await _appointmentService.GetAppointmentsByBranch(branchId);
            return Ok(appointments);
        }

        [Authorize(Policy = Policies.AdminOrRecepcionista)]
        [HttpGet("by-date")]
        public async Task<ActionResult<List<AppointmentResponse>>> GetByDate([FromQuery] DateTime day, [FromQuery] Guid? branchId = null)
        {
            return Ok(await _appointmentService.GetAppointmentsByBranchAndDate(day, branchId));
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpGet("/api/appointments/schedule")]
        public async Task<ActionResult<BranchAgendaResponse>> GetSchedule([FromQuery] Guid branchId, [FromQuery] DateTime date)
        {
            return Ok(await _appointmentService.GetBranchAgenda(branchId, date));
        }

        [Authorize(Policy = Policies.SysAdmin)]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var appointment = await _appointmentService.GetById(id);
            return Ok(appointment);
        }

        [AllowAnonymous]
        [HttpGet("/api/appointments/available-slots")]
        public async Task<ActionResult<List<AvailableSlotResponse>>> GetAvailableSlots(
            [FromQuery] Guid branchId,
            [FromQuery] Guid staffId,
            [FromQuery] Guid serviceId,
            [FromQuery] DateTime date)
        {
            var slots = await _appointmentService.GetAvailableSlots(branchId, staffId, serviceId, date);
            return Ok(slots);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AppointmentRequest request)
        {
            var appointment = await _appointmentService.CreateAppointment(request);
            return Ok(appointment);
        }



        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] AppointmentRequest request)
        {
            var appointment = await _appointmentService.UpdateAppointment(id, request);
            return Ok(appointment);
        }

        [Authorize(Policy = Policies.SysAdminOrAdminOrRecepcionista)]
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] UpdateAppointmentStatusRequest request)
        {
            var appointment = await _appointmentService.UpdateStatus(id, request.Status);
            return Ok(appointment);
        }


        [Authorize(Policy = Policies.SysAdminOrAdmin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _appointmentService.DeleteAppointment(id);
            return NoContent();
        }
    }
}
