using System.Security.Claims;
using GestionTurnos.Application.Abstraction.Infrastructure;
using Microsoft.AspNetCore.Http;


namespace GestionTurnos.Infrastructure.ExternalServices
{
    public class TenantProvider : ITenantProvider
    {
        // Otra vez perdidad princesa?
        // Veni veni que te explico, aca agarramos el BusinessId del token JWT.En caso de que tenga, sino devolvemos Null.
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetBusinessId()
        {
            var claimValue = _httpContextAccessor.HttpContext?.User?.FindFirst("BusinessId")?.Value;
            
            if (Guid.TryParse(claimValue, out Guid businessId))
            {
                return businessId;
            }
            
            return null;
        }

        public Guid? GetBranchId()
        {
            var claimValue = _httpContextAccessor.HttpContext?.User?.FindFirst("BranchId")?.Value;
            
            if (Guid.TryParse(claimValue, out Guid branchId))
            {
                return branchId;
            }
            
            return null;
        }

        public Guid? GetUserId()
        {
            var claimValue = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            
            if (Guid.TryParse(claimValue, out Guid userId))
            {
                return userId;
            }
            
            return null;
        }

        public string? GetUserRole()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
        }
    } 
}