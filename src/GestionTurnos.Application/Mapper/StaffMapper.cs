using GestionTurnos.Application.Request;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Mapper
{
    public static class StaffMapper
    {
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
            };
        }

       
    }
}