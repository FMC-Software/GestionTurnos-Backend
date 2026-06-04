using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Abstraction.Infrastructure.Auth;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestionTurnos.Infrastructure.ExternalServices
{
    public class AuthService : IAuthService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IBusinessSubscriptionRepository _BusinessSubscriptionRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IBranchService _branchService;
        private readonly IEmailContentBuilder _emailContentBuilder;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IScheduleService _scheduleService;


        public AuthService(IStaffRepository staffRepository, 
            IPlanRepository planRepository, 
            IBusinessSubscriptionRepository BusinessSubscriptionRepository,
            IConfiguration configuration, IEmailService emailService,
            IEmailContentBuilder emailContentBuilder,
            IBranchService branchService,   
            IScheduleService scheduleService,
            IHttpContextAccessor httpContextAccessor)
        {
            _staffRepository = staffRepository;
            _planRepository = planRepository;
            _BusinessSubscriptionRepository = BusinessSubscriptionRepository;
            _configuration = configuration;
            _emailService = emailService;
            _branchService = branchService;
            _emailContentBuilder = emailContentBuilder;
            _httpContextAccessor = httpContextAccessor;
            _scheduleService = scheduleService;
        }

        public AuthResponse? SignUp(SignUpRequest request)
        {
            bool emailExists = _staffRepository.GetAllGlobal().Any(s => s.Email == request.Email);
            if (emailExists)
            {
                throw new ConflictException("El correo electrónico ya está registrado.");
            }

            
            if (!Enum.TryParse<TypeBusiness>(request.BusinessCategory, ignoreCase: true, out var typeBusinessParsed))
            {
                //throw new BadRequestException($"La categoría de negocio '{request.BusinessCategory}' no es válida.");
            }

            if(request.Plan == null || request.Plan.Id == Guid.Empty)
            {
                request.Plan = _planRepository.GetAllGlobal().FirstOrDefault(p => p.Name == "Free Plan")
                    ?? _planRepository.GetAllGlobal().FirstOrDefault(); // Agarra el primero si no hay "Free Plan"
                    
                if (request.Plan == null) 
                    throw new ConflictException("No hay ningún plan cargado en la base de datos.");
            }
            else
            {
                request.Plan = _planRepository.GetAllGlobal().FirstOrDefault(p => p.Id == request.Plan.Id)
                    ?? throw new ConflictException("El plan especificado no existe.");
            }

            var newBusiness = new Business
            {
                Id = Guid.NewGuid(),
                Name = $"{request.Name} - {request.BusinessCategory}",
                Url = $"http://www.{request.Name.Replace(" ", "")}.FCMTurniFy.com",
                TypeBusiness = typeBusinessParsed
            };

            var BusinessSubscription = new BusinessSubscription
            {
                Id = Guid.NewGuid(),
                BusinessId = newBusiness.Id,
                Business = newBusiness,
                PlanId = request.Plan.Id,
                Plan = request.Plan,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow + TimeSpan.FromDays(request.Plan.DurationDays),
                Status = Status.Active
            };

            var newBranch = _branchService.CreateInitialBranch(request, newBusiness);

            var newStaff = request.ToRegisterNewBusinessAndStaff(newBusiness, newBranch);
            _staffRepository.Add(newStaff);
            _BusinessSubscriptionRepository.Add(BusinessSubscription);

            return new AuthResponse
            {
                
                Token = GenerarToken(newStaff.Id, newStaff.Name, newStaff.Rol, newBusiness.Id, newBranch.Id),
            };
        }

        public AuthResponse? SignIn(SignInRequest request)
        {
            var user = _staffRepository.GetAllGlobal().FirstOrDefault(s => s.Email == request.Email);
            if (user == null)
            {
                throw new ConflictException("Credenciales Incorrectas."); 
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return null;
            }

            return new AuthResponse
            {
                
                Token = GenerarToken(user.Id, user.Name, user.Rol, user.BusinessId, user.BranchId),
            };
        }

        public void ForgotPassword(string request)
        {
            var user = _staffRepository.GetAllGlobal().FirstOrDefault(s => s.Email == request);
            if (user == null)
            {
                throw new ConflictException("No se encontró un usuario con ese correo electrónico.");
            }

           var  Token = GenerateTokerForgotPassword(user);

           var emailMessage = _emailContentBuilder.BuildResetPassword(user,Token);


            // Aquí deberías enviar el correo utilizando tu servicio de email
            _emailService.SendEmailAsync(emailMessage);
 
        }

        public void ResetPassword(string request, string token) // MICAEL MIRA ESTO MAÑANA _-------------------_
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken;

            try
            {
                // 1. Leemos el token JWT que llegó por parámetro desde la URL
                jwtToken = tokenHandler.ReadJwtToken(token);
            }
            catch
            {
                throw new ConflictException("El enlace de recuperación no es válido o ha expirado.");
            }


            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var updateDateTimeClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UpdateDateTime")?.Value;


            var userEntity = _staffRepository.GetAllGlobal().FirstOrDefault(s => s.Id == Guid.Parse(userId));
            if (userEntity == null)
            {
                throw new ConflictException("No se encontró el usuario asociado a esta solicitud.");
            }


            if (userEntity.UpdateDateTime.Ticks.ToString() != updateDateTimeClaim)
            {
                throw new ConflictException(
                    "Este enlace de recuperación ya no es válido.");
            }

            if (BCrypt.Net.BCrypt.Verify(request, userEntity.Password))
            {
                throw new ConflictException("La nueva contraseña no puede ser igual a la contraseña anterior.");
            }


            userEntity.Password = BCrypt.Net.BCrypt.HashPassword(request);


            userEntity.Password = request;
            userEntity.UpdateDateTime = DateTime.UtcNow;

            _staffRepository.Update(userEntity);
        }


        private string GenerarToken(Guid userId, string nameStaff, Rol? rol, Guid? businessId, Guid? branchId = null)
        {
            string key = _configuration["Jwt:Key"]!;
            string issuer = _configuration["Jwt:Issuer"]!;
            string audience = _configuration["Jwt:Audience"]!;
            int expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"]!);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), 
                new Claim(JwtRegisteredClaimNames.Name, nameStaff),
                new Claim(ClaimTypes.Role, rol?.ToString() ?? "SysAdmin"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // 2. DETALLE CLAVE: El claim BusinessId solo se inyecta si existe un ID de negocio asociado.
            // Si el rol es Sysadmin, businessId llegará como null y se omitirá de forma limpia y segura.
            if (businessId.HasValue && businessId.Value != Guid.Empty)
            {
                claims.Add(new Claim("BusinessId", businessId.Value.ToString()));
            }

            if (branchId.HasValue && branchId.Value != Guid.Empty)
            {
                claims.Add(new Claim("BranchId", branchId.Value.ToString()));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateTokerForgotPassword(Staff User)
        {
            string key = _configuration["Jwt:Key"]!;
            string issuer = _configuration["Jwt:Issuer"]!;
            string audience = _configuration["Jwt:Audience"]!;
            int expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"]!);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var claims = new List<Claim>
{
            new Claim(JwtRegisteredClaimNames.Sub, User.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, User.Email),
            new Claim("UpdateDateTime", User.UpdateDateTime.Ticks.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
};

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}