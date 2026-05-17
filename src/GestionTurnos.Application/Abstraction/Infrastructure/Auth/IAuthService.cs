using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Abstraction.Infrastructure.Auth
{
    public interface IAuthService
    {
        AuthResponse? SignUp(SignUpRequest request);
        AuthResponse? SignIn(SignInRequest request);
    }
}
