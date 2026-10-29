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
    public class HelpContentController : ControllerBase
    {
        private readonly IHelpContentService _helpContentService;

        public HelpContentController(IHelpContentService helpContentService)
        {
            _helpContentService = helpContentService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<HelpContentResponse>> Get()
        {
            return Ok(await _helpContentService.Get());
        }

        [HttpPut]
        public async Task<ActionResult<HelpContentResponse>> Update([FromBody] HelpContentRequest request)
        {
            return Ok(await _helpContentService.Update(request));
        }

        [HttpPost("faqs")]
        public async Task<ActionResult<FaqResponse>> AddFaq([FromBody] FaqRequest request)
        {
            return Ok(await _helpContentService.AddFaq(request));
        }

        [HttpPut("faqs/{faqId:guid}")]
        public async Task<ActionResult<FaqResponse>> UpdateFaq([FromRoute] Guid faqId, [FromBody] FaqRequest request)
        {
            return Ok(await _helpContentService.UpdateFaq(faqId, request));
        }

        [HttpDelete("faqs/{faqId:guid}")]
        public async Task<ActionResult> DeleteFaq([FromRoute] Guid faqId)
        {
            await _helpContentService.DeleteFaq(faqId);
            return NoContent();
        }
    }
}
