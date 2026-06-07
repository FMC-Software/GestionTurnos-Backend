using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Abstraction.Infrastructure.Auth;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Application.Services;
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
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IBranchService _branchService;
        private readonly IEmailContentBuilder _emailContentBuilder;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IScheduleService _scheduleService;
        private readonly ISysAdminService _sysAdminService;
        private readonly IBusinessService _businessService;
        private readonly IBusinessSubscriptionService _businessSubscriptionService;
        private readonly IStaffService _staffService;
        private readonly IPlanService _planService;

        public AuthService(IStaffRepository staffRepository, 
            IConfiguration configuration, IEmailService emailService,
            IEmailContentBuilder emailContentBuilder,
            IBranchService branchService,   
            IScheduleService scheduleService,
            IHttpContextAccessor httpContextAccessor,
            ISysAdminService sysAdminService,
            IBusinessService businessService,
            IBusinessSubscriptionService businessSubscriptionService,
            IStaffService staffService,
            IPlanService planService)
            

        {
            _staffRepository = staffRepository;
            _configuration = configuration;
            _emailService = emailService;
            _branchService = branchService;
            _emailContentBuilder = emailContentBuilder;
            _httpContextAccessor = httpContextAccessor;
            _scheduleService = scheduleService;
            _sysAdminService = sysAdminService;
            _businessService = businessService;
            _businessSubscriptionService = businessSubscriptionService;
            _staffService = staffService;
            _planService = planService;

        }

        public AuthResponse? SignUp(SignUpRequest request)
        {
            bool emailExists = _staffService.GetByEmail(request.Email) != null;
            if (emailExists)
            {
                throw new ConflictException("El correo electrónico ya está registrado.");
            }

            
            if (!Enum.TryParse<TypeBusiness>(request.BusinessCategory, ignoreCase: true, out var typeBusinessParsed)) // MICAEL ARREGLA ESTO+
            {
                //throw new BadRequestException($"La categoría de negocio '{request.BusinessCategory}' no es válida.");
            }


            var selectedPlan = _planService.GetPlanOrDefault(request.Plan?.Id);

            var newBusiness = _businessService.initialBusiness(request, typeBusinessParsed);

            var newBranch = _branchService.CreateInitialBranch(request, newBusiness);

            var newStaff = request.ToRegisterNewBusinessAndStaff(newBusiness, newBranch);
            _staffRepository.Add(newStaff);
            _businessSubscriptionService.InitialBusinessSubscription(selectedPlan,newBusiness);
            

            return new AuthResponse
            {
                
                Token = GenerarToken(newStaff.Id, newStaff.Name, newStaff.Rol, newBusiness.Id, newBranch.Id),
            };
        }

        public AuthResponse? SignIn(SignInRequest request)
        {
            var user = _staffService.GetByEmail(request.Email);

            var sysAdmin = _sysAdminService.GetByEmail(request.Email);

            // No existe ningún usuario
            if (user == null && sysAdmin == null)
            {
                throw new ConflictException("Credenciales Incorrectas.");
            }

            // Login Staff
            if (user != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    throw new ConflictException("Credenciales Incorrectas.");
                }

                return new AuthResponse
                {
                    Token = GenerarToken(
                        user.Id,
                        user.Name,
                        user.Rol,
                        user.BusinessId,
                        user.BranchId)
                };
            }

            // Login SysAdmin
            if (!BCrypt.Net.BCrypt.Verify(request.Password, sysAdmin.Password))
            {
                throw new ConflictException("Credenciales Incorrectas.");
            }

            return new AuthResponse
            {
                Token = GenerarToken(
                    sysAdmin.Id,
                    sysAdmin.Name,
                    null,
                    null,
                    null)
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