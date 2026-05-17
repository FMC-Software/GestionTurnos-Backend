using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Abstraction.Infrastructure.Auth;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;
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
        private readonly IBranchRepository _branchRepository;

        public AuthService(IStaffRepository staffRepository, IConfiguration configuration, IBranchRepository branchRepository)
        {
            _staffRepository = staffRepository;
            _configuration = configuration;
            _branchRepository = branchRepository;
        }

        //Registro de un nuevo negocio y su staff asociado y devuelvo un token JWT para el nuevo staff registrado.
        public AuthResponse? SignUp(SignUpRequest request)
        {
            //validemos que el correo electrónico no esté registrado en la base de datos
            bool emailExists = _staffRepository.GetAll().Any(s => s.Email == request.Email);
            if (emailExists) {
                throw new Exception("El correo electrónico ya está registrado.");
            }

            // Al momento de registrar un nuevo negocio, se crea automáticamente un nuevo staff asociado a ese negocio con el rol de admin
            // y una branch principal con la dirección y teléfono proporcionados en el request.
            var newBusiness = new Business
            {
                Id = Guid.NewGuid(),
                Name = $"{request.Name} - {request.BusinessCategory}",
                Url = $"http://www.{request.Name.Replace(" ", "")}.FCMTurniFy.com"
            };

            var newBranch = new Branch
            {
                Id = Guid.NewGuid(),
                Address = request.Address,
                Phone = request.BranchPhone,
                BusinessId = newBusiness.Id,
                City= request.City,
                Business = newBusiness
            };

            var newStaff = request.ToRegisterNewBusinessAndStaff(newBusiness);
            _branchRepository.Add(newBranch);
            _staffRepository.Add(newStaff);

            return new AuthResponse
            {
                Token = GenerarToken(newStaff.Id, newStaff.Name, newStaff.Rol),

            };
        }

        public AuthResponse? SignIn(SignInRequest request)
        {
            Guid userId;
            Rol rol;
            string contrasenaHasheada;

            var user = _staffRepository.GetAll().FirstOrDefault(s => s.Email == request.Email);
            if (user == null)
            {
                throw new Exception("Credenciales Incorrectas.");
             }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return null;

            userId = user.Id;
            contrasenaHasheada = user.Password;
            rol = user.Rol;
            return new AuthResponse
            {
                Token = GenerarToken(userId, user.Name, rol),
            
            };
        }

        private string GenerarToken(Guid Idbusiness, string NameStaff, Rol Rol)
        {
            string key = _configuration["Jwt:Key"]!;
            string issuer = _configuration["Jwt:Issuer"]!;
            string audience = _configuration["Jwt:Audience"]!;
            int expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"]!);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, Idbusiness.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, NameStaff),
                new Claim(ClaimTypes.Role, Rol.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
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
