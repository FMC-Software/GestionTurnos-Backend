using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;
using System.ComponentModel.DataAnnotations;

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

            };
        }

        public static BusinessSummaryResponse ToSummaryResponse(this Business business)
        {
            return new BusinessSummaryResponse
            {
                Id = business.Id,
                Name = business.Name
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
                DurationMinutes = service.Duration,
                Description = service.Description
            };
        }

        public static void ToUpdateBusiness(this Business business, BusinessUpdateRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Name))
                business.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Url))
                business.Url = request.Url;

            if (!string.IsNullOrWhiteSpace(request.LogoUrl))
                business.UrlLogo = request.LogoUrl;

            if (request.IsActive.HasValue)
                business.IsActive = request.IsActive.Value;
        }

    }
}