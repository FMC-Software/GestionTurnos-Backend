using GestionTurnos.Application.Request;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Mapper
{
    public static class StaffMapper
    {
        // Para cuando el negocio ya existe (Método 1)
        public static Staff ToStaff(this StaffRequest staffRequest, Guid businessId)
        {
            return new Staff
            {
                Id = Guid.NewGuid(),
                Name = staffRequest.Name,
                Email = staffRequest.Email,
                Password = staffRequest.Password,
                Phone = staffRequest.Phone,
                Rol = staffRequest.Rol,
                LinkPhoto = staffRequest.LinkPhoto,
                BusinessId = businessId
                // Business se deja nulo o se cargará por lazy loading/include si es necesario
            };
        }

        // Para cuando creas el negocio en caliente (Método 2)
        public static Staff ToStaff(this StaffRequest staffRequest, Business newBusiness)
        {
            return new Staff
            {
                Id = Guid.NewGuid(),
                Name = staffRequest.Name,
                Email = staffRequest.Email,
                Password = staffRequest.Password,
                Phone = staffRequest.Phone,
                Rol = staffRequest.Rol,
                LinkPhoto = staffRequest.LinkPhoto,
                BusinessId = newBusiness.Id,
                Business = newBusiness
            };
        }
    }
}