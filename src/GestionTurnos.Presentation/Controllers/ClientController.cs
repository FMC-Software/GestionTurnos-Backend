using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTurnos.Presentation.Controllers
{
    [Authorize(Policy = Policies.SysAdminOrAdminOrRecepcionista)]

    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [Authorize(Policy = Policies.SysAdminOrAdminOrRecepcionista)]

        [HttpGet]
        public async Task<ActionResult<List<ClientsResponse>>> GetAll()
        {
            return Ok(await _clientService.GetClientsOfCurrentBusiness());
        }


        [Authorize(Policy = Policies.SysAdminOrAdminOrRecepcionista)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientsResponse>> GetById([FromRoute] Guid id)
        {
            return Ok(await _clientService.GetById(id));
        }

        [Authorize(Policy = Policies.SysAdminOrAdminOrRecepcionista)]
        [HttpGet("search")]
        public async Task<ActionResult<ClientsResponse>> GetByName([FromQuery] string name)
        {
            return Ok(await _clientService.GetByName(name));
        }

        [HttpPost]
        public async Task<ActionResult<ClientsResponse>> Create([FromBody] ClientRequest request)
        {
            var newClient = await _clientService.CreateClient(request);
            return CreatedAtAction(nameof(GetById), new { id = newClient.Id }, newClient);
        }

        [Authorize(Policy = Policies.SysAdminOrAdminOrRecepcionista)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromBody] ClientRequest request, [FromRoute] Guid id)
        {
            await _clientService.UpdateClient(request, id);
            return NoContent();
        }

        [Authorize(Policy = Policies.SysAdminOrAdminOrRecepcionista)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            await _clientService.DeleteClient(id);
            return NoContent();
        }
    }
}
