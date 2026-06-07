using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction
{
    public interface IStaffService
    {
        List<StaffsResponse> GetStaffOfCurrentBusiness();
        StaffsResponse GetById(Guid id);
        StaffsResponse CreateStaff(StaffRequest request);
        StaffsResponse UpdateStaff(StaffRequest staff, Guid idStaff);
        void DeleteStaff(Guid id);
        List<GlobalStaffResponse> GetAllGlobal();

        Staff GetByEmail(string email);

        Staff GetByEmailGlobal(string email);
    }
}