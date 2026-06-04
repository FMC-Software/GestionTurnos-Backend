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
            service.Name = request.Name;
            service.Categoria = request.Categoria;
            service.Description = request.Description;
            service.Duration = request.Duration;
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
