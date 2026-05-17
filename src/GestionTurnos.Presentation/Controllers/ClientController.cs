using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GestionTurnos.Presentation.Controllers
{
    public class ClientController : ControllerBase   
    {

       private readonly IClientService _clientService;
        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public ActionResult<List<Client>> GetAllClients()
        {
            
            return Ok(_clientService.GetAll());
        }

        [HttpGet("Business/{businessId}")]
        public ActionResult<List<Client>> GetClientsOfBusiness(Guid businessId)
        {

            return Ok(_clientService.GetClientsOfBusiness(businessId));
        }
    }
}
