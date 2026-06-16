using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Response;
using GestionTurnos.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestionTurnos.Presentation.Controllers
{
    [Authorize(Policy = Policies.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public ActionResult<DashboardResponse> GetDashboard()
        {
            try
            {
                var dashboard = _dashboardService.GetDashboard();

                return Ok(dashboard);
            }
            catch (ConflictException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "error interno del servidor");
            }
        }
    }
}
