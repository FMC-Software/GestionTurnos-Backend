using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Request;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GestionTurnos.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
       private readonly IScheduleService _scheduleService;

        public  ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        
        [HttpPut("{id}")]
        public void Put([FromBody] ScheduleRequest request, Guid id)
        {
            _scheduleService.UpdateSchedule(request, id);
        }

    }
} 
