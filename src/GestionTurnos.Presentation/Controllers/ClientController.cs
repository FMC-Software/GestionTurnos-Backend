using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTurnos.Presentation.Controllers
{
    [Authorize(Roles = Policies.Admin)]
    [Authorize(Roles = Policies.Recepcionista)]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpGet]
        public ActionResult<List<ClientsResponse>> GetAll()
        {
            return Ok(_clientService.GetClientsOfCurrentBusiness());
        }


        [Authorize(Policy = Policies.Admin)]
        [Authorize(Policy = Policies.Recepcionista)]
        [HttpGet("{id}")]
        public ActionResult<ClientsResponse> GetById([FromRoute] Guid id)
        {
            return Ok(_clientService.GetById(id));
        }

        [Authorize(Policy = Policies.Admin)]
        [Authorize(Policy = Policies.Recepcionista)]
        [HttpGet("search")]
        public ActionResult<ClientsResponse> GetByName([FromQuery] string name)
        {
            return Ok(_clientService.GetByName(name));
        }

        [HttpPost]
        public ActionResult<ClientsResponse> Create([FromBody] ClientRequest request)
        {
            var newClient = _clientService.CreateClient(request);
            return CreatedAtAction(nameof(GetById), new { id = newClient.Id }, newClient);
        }

        [Authorize(Policy = Policies.Admin)]
        [Authorize(Policy = Policies.Recepcionista)]
        [HttpPut("{id}")]
        public ActionResult Update([FromBody] ClientRequest request, [FromRoute] Guid id)
        {
            _clientService.UpdateClient(request, id);
            return NoContent();
        }

        [Authorize(Policy = Policies.Admin)]
        [Authorize(Policy = Policies.Recepcionista)]
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] Guid id)
        {
            _clientService.DeleteClient(id);
            return NoContent();
        }
    }
}