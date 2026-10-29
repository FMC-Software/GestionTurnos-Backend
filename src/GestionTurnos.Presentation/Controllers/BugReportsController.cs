using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTurnos.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BugReportsController : ControllerBase
    {
        private readonly IBugReportService _bugReportService;

        public BugReportsController(IBugReportService bugReportService)
        {
            _bugReportService = bugReportService;
        }

        // Cualquier usuario autenticado del negocio puede reportar.
        [Authorize(Policy = Policies.AnyStaff)]
        [HttpPost]
        public async Task<ActionResult<BugReportResponse>> Create([FromBody] BugReportCreateRequest request)
        {
            return Ok(await _bugReportService.Create(request));
        }

        [Authorize(Policy = Policies.SysAdmin)]
        [HttpGet]
        public async Task<ActionResult<List<BugReportResponse>>> GetAll()
        {
            return Ok(await _bugReportService.GetAll());
        }

        [Authorize(Policy = Policies.SysAdmin)]
        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<BugReportResponse>> UpdateStatus([FromRoute] Guid id, [FromBody] BugReportStatusUpdateRequest request)
        {
            return Ok(await _bugReportService.UpdateStatus(id, request));
        }
    }
}
