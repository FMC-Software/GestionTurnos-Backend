using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionTurnos.Application.Abstraction.Infrastructure.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse?> SignUp(SignUpRequest request);
        Task<AuthResponse?> SignIn(SignInRequest request);

        public Task ForgotPassword(string request);

        public Task ResetPassword(string request, string token);
    }
}
