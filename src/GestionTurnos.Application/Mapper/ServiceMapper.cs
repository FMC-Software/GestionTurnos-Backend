using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Mapper
{
    public static class ServiceMapper
    {
        public static Service ToService(this ServiceRequest request, Guid businessId)
        {
            return new Service
            {
                BusinessId = businessId,
                Name = request.Name,
                Categoria = request.Categoria,
                Description = request.Description,
                Duration = request.Duration,
                Price = request.Price
            };
        }

        public static void UpdateFromRequest(this Service service, ServiceRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Name))
                service.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Categoria))
                service.Categoria = request.Categoria;

            if (!string.IsNullOrWhiteSpace(request.Description))
                service.Description = request.Description;

            if (request.Duration > 0)
                service.Duration = request.Duration;

            if (request.Price > 0)
                service.Price = request.Price;

            service.UpdateDateTime = DateTime.UtcNow;
        }

        public static ServiceBusinessResponse ToServiceResponse(this Service service)
        {
            return new ServiceBusinessResponse
            {
                Id = service.Id,
                BusinessId = service.BusinessId,
                Name = service.Name,
                Categoria = service.Categoria,
                Description = service.Description,
                Duration = service.Duration,
                Price = service.Price
            };
        }
    }
}
