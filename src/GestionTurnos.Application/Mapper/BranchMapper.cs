using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Mapper
{
    public static class BranchMapper
    {
        public static Branch ToBranch(this CreateBranchRequest request)
        {
            return new Branch
            {
                BusinessId = request.BusinessId,
                Name = request.Name,
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
            branch.UpdateDateTime = DateTime.Now;
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
    }
}

