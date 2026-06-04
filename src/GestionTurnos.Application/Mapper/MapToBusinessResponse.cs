using GestionTurnos.Domain.Entities;
using GestionTurnos.Application.Response;

namespace GestionTurnos.Application.Mapper
{
    public static class BusinessMapper
    {
        public static BusinessDashboardResponse ToResponse(this Business business)
        {
            return new BusinessDashboardResponse
            {
                Id = business.Id,
                Name = business.Name,
                Url = business.Url,
                LogoUrl = business.UrlLogo ?? string.Empty,
                Branches = business.Branches.Select(b => b.ToResponse()).ToList(),
                Services = business.Services.Select(s => s.ToResponse()).ToList(),
                Clients = business.Clients.Select(c => ClientMapper.ToResponse(c)).ToList()
            };
        }

        public static BranchResponse ToResponse(this Branch branch)
        {
            return new BranchResponse
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                Phone = branch.Phone,
                City = branch.City
            };
        }

        public static ServiceResponse ToResponse(this Service service)
        {
            return new ServiceResponse
            {
                Id = service.Id,
                Name = service.Name,
                Price = service.Price,
                DurationMinutes = service.Duration
            };
        }

      
    }
}