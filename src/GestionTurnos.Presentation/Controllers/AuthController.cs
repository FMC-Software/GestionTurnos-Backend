using GestionTurnos.Application.Abstraction.Infrastructure.Auth;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using Microsoft.AspNetCore.Mvc;


namespace GestionTurnos.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Signup")]
        public ActionResult<AuthResponse> Authenticate([FromBody] SignUpRequest credentials)
        {

                return Ok(_authService.SignUp(credentials));

        }

        [HttpPost("Signin")]
        public ActionResult<AuthResponse> Authorize([FromBody] SignInRequest credentials)
        {

            
                return Ok(_authService.SignIn(credentials));
        }

    }
}
