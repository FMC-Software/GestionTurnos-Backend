using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction
{
    public interface IStaffService
    {
        Task<List<StaffsResponse>> GetStaffOfCurrentBusiness();
        Task<StaffsResponse> GetById(Guid id);
        Task<StaffsResponse> CreateStaff(StaffRequest request);
        Task<StaffsResponse> UpdateStaff(StaffRequest staff, Guid idStaff);
        Task DeleteStaff(Guid id);
        Task<List<GlobalStaffResponse>> GetAllGlobal();

        Task<Staff?> GetByEmail(string email);

        Task<Staff?> GetByEmailGlobal(string email);

        Task<List<StaffSummaryResponse>> GetStaffByBranchId(Guid branchId);

        Task<StaffsResponse> GetAdminOfCurrentBusiness();

        Task<StaffsResponse> UpdateStaffByEmail(UpdateStaffRequest request);
    }
}
