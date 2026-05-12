using GestionTurnos.Application.Abstraction;
using GestionTurnos.Domain.Entities;
using Microsoft.AspNetCore.Mvc;



namespace GestionTurnos.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessService _businessService;

        public BusinessController(IBusinessService businessService)
        {
            _businessService = businessService;
        }

        [HttpGet]
        public ActionResult<Business> GetAll()
        {

            return Ok(_businessService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Business> GetById(Guid id)
        {
            return Ok(_businessService.GetById(id));
        }

        [HttpPost]
        public ActionResult<Business> Post([FromBody] Business value)
        {
            return Ok(_businessService.Create(value));
        }

        [HttpPut("{id}")]
        public ActionResult<bool> Update(Guid id, [FromBody] string value)
        {
            return Ok(true);
        }

        [HttpDelete("{id}")]
        public ActionResult<bool> Delete(Guid id)
        {
            _businessService.Delete(id);
            return Ok(true);
        }
    }
}
