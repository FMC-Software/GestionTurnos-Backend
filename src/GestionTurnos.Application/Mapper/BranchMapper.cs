using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Mapper
{
    public static class BranchMapper
    {
        public static Branch ToBranch(this CreateBranchRequest request, Guid businessId)
        {
            return new Branch
            {
                BusinessId = businessId,
                Name = request.Name ?? "Sucursal 01",
                Address = request.Address,
                Phone = request.Phone,
                City = request.City
            };
        }

        public static void UpdateFromRequest(this Branch branch, CreateBranchRequest request)
        {
            branch.Name = request.Name;
            branch.Address = request.Address;
            branch.Phone = request.Phone;
            branch.City = request.City;
            branch.UpdateDateTime = DateTime.UtcNow;
        }

        public static BranchResponse ToBranchResponse(this Branch branch)
        {
            return new BranchResponse
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                Phone = branch.Phone ?? string.Empty,
                City = branch.City
            };
        }

        public static InfoBranchResponse ToInfoBranchResponse(this Branch branch)
        {
            if (branch == null) return null;

            return new InfoBranchResponse
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                Phone = branch.Phone,
                City = branch.City,
                BusinessId = branch.BusinessId,

                // Mapeamos las listas internas usando los mappers de cada entidad relacionada
                Schedules = branch.Schedules.Select(s => s.ToResponseSchedule()).ToList(),
                Staff = branch.Staff.Select(st => st.ToResponse()).ToList(),
                Services = branch.Services.Select(sr => sr.ToServiceResponse()).ToList()
            };
        }
    }
}

