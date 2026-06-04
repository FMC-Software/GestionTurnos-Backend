using GestionTurnos.Application.Abstraction.Infrastructure.Auth;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using Microsoft.AspNetCore.Identity.Data;
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


        [HttpPost("forgotPassword")]

        public ActionResult forgotPassword([FromBody] string request)
        {

                _authService.ForgotPassword(request);
                return Ok("Correo de recuperación de contraseña enviado exitosamente.");

        }

        [HttpPost("resetPassword")]

        public ActionResult resetPassword([FromBody] string request, [FromQuery] string token)
        {

                _authService.ResetPassword(request, token);
                return Ok("Contraseña restablecida exitosamente.");
          

        }

    }
}
