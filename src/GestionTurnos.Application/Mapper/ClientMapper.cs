using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Mapper
{
    public static class ClientMapper
    {
        public static Client ToEntity(this ClientRequest request)
        {
            return new Client
            {
                Id = Guid.NewGuid(), 
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                IsDeleted = false
            };
        }

        
        public static ClientsResponse ToResponse(this Client entity)
        {
            return new ClientsResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Phone = entity.Phone
            };
        }

     
        public static GlobalClientResponse ToGlobalResponse(this Client entity)
        {
            return new GlobalClientResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Phone = entity.Phone,
                BusinessId = entity.BusinessId,
                BusinessName = entity.Business != null ? entity.Business.Name : null
            };
        }

        
        public static void UpdateFromDto(this Client entity, ClientRequest request)
        {
            entity.Name = request.Name;
            entity.Email = request.Email;
            entity.Phone = request.Phone;
        }
    }
}