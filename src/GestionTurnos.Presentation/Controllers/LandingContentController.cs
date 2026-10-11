using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTurnos.Presentation.Controllers
{
    [Authorize(Policy = Policies.SysAdmin)]
    [Route("api/[controller]")]
    [ApiController]
    public class LandingContentController : ControllerBase
    {
        private readonly ILandingContentService _landingContentService;

        public LandingContentController(ILandingContentService landingContentService)
        {
            _landingContentService = landingContentService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<LandingContentResponse>> Get()
        {
            return Ok(await _landingContentService.Get());
        }

        [HttpPut]
        public async Task<ActionResult<LandingContentResponse>> Update([FromBody] LandingContentRequest request)
        {
            return Ok(await _landingContentService.Update(request));
        }
    }
}
