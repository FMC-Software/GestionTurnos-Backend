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
        private readonly ISysAdminDashboardService _sysAdminDashboardService;
        private readonly ISysAdminService _sysAdminService;

        public SysAdminController(IStaffService staffService, ISysAdminDashboardService sysAdminDashboardService, ISysAdminService sysAdminService)
        {
            _staffService = staffService;
            _sysAdminDashboardService = sysAdminDashboardService;
            _sysAdminService = sysAdminService;
        }

        [Authorize(Policy = "SysAdmin")]
        [HttpGet("businesses")]
        public async Task<ActionResult<List<BusinessCardResponse>>> GetBusinesses()
        {
            return Ok(await _sysAdminService.GetBusinessCards());
        }

        [Authorize(Policy = "SysAdmin")]
        [HttpGet("businesses/{businessId:guid}")]
        public async Task<ActionResult<BusinessDetailResponse>> GetBusinessDetail([FromRoute] Guid businessId)
        {
            return Ok(await _sysAdminService.GetBusinessDetail(businessId));
        }

        [Authorize(Policy = "SysAdmin")]
        [HttpGet("dashboard")]
        public async Task<ActionResult<SysAdminDashboardResponse>> GetDashboard()
        {
            return Ok(await _sysAdminDashboardService.GetDashboard());
        }

        [Authorize(Policy = "SysAdmin")]
        [HttpGet]
        public async Task<ActionResult<List<GlobalStaffResponse>>> GetAll()
        {

            return Ok(await _staffService.GetAllGlobal());
        }

        [Authorize(Policy = "SysAdmin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<GlobalStaffResponse>> GetById(Guid id)
        {

            return Ok(await _staffService.GetById(id));
        }

        [Authorize(Policy = "SysAdmin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Staff>> UpdateStaff([FromBody] StaffRequest Staff, [FromRoute] Guid id)
        {
            var updatedUser = await _staffService.UpdateStaff(Staff, id);
            return Ok(updatedUser);
        }
    }
}
