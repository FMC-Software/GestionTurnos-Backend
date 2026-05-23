using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionTurnos.Presentation.Controllers
{
    [Authorize]
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
        public ActionResult<List<Client>> GetAllClients()
        {
            
            return Ok(_clientService.GetAll());
        }
        [Authorize(Policy = Policies.Admin)]
        [HttpGet("Business/{businessId}")]
        public ActionResult<List<Client>> GetClientsOfBusiness(Guid businessId)
        {

            return Ok(_clientService.GetClientsOfBusiness(businessId));
        }
    }
}
