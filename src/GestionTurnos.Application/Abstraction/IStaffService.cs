

using GestionTurnos.Application.Request;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction
{
    public interface IStaffService
    {
        public List<Staff> GetAll();
        public Staff GetById(Guid id);

        public Staff CreateStaffWhitBusiness(StaffRequest request);

        public Staff CreateStaff(StaffRequest request, Guid Id_Business); 

        public Staff UpdateStaff(StaffRequest staff, Guid idStaff);
        public void DeleteStaff(Guid id);

    }
}
