using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Mapper
{
    public static class StaffMapper
    {
        public static Staff ToStaff(this StaffRequest staffRequest)
        {
            return new Staff
            {
                Id = Guid.NewGuid(),
                Name = staffRequest.Name,
                Email = staffRequest.Email,
                Password = staffRequest.Password,
                Phone = staffRequest.Phone,
                Rol = staffRequest.Rol,
                LinkPhoto = staffRequest.LinkPhoto ?? string.Empty,
                BranchId = staffRequest.BranchId ?? Guid.Empty
            };
        }

        public static StaffsResponse ToResponse(this Staff entity)
        {
            return new StaffsResponse
            {
                Id = entity.Id,
                StaffName = entity.Name,
                StaffEmail = entity.Email,
                StaffPhone = entity.Phone,
                Rol = entity.Rol,
                StaffLinkPhoto = entity.LinkPhoto,
                BranchId = entity.BranchId,
                BranchName = entity.Branch?.Name ?? string.Empty,
            };
        }

        public static GlobalStaffResponse ToGlobalResponse(this Staff entity)
        {
            return new GlobalStaffResponse
            {
                Id = entity.Id,
                StaffName = entity.Name,
                StaffEmail = entity.Email,
                StaffPhone = entity.Phone,
                Rol = entity.Rol,
                StaffLinkPhoto = entity.LinkPhoto,
                BusinessId = entity.BusinessId,
                BusinessName = entity.Business != null ? entity.Business.Name : "Desconocido"
            };
        }

        public static void UpdateFromDto(this Staff entity, StaffRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Name))
                entity.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Email))
                entity.Email = request.Email;

            if (!string.IsNullOrWhiteSpace(request.Phone))
                entity.Phone = request.Phone;

            if (!string.IsNullOrWhiteSpace(request.LinkPhoto))
                entity.LinkPhoto = request.LinkPhoto;

                entity.Rol = request.Rol;
           
        }
    }
}