using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Mapper;
using GestionTurnos.Application.Request;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Services
{
    public class StaffService : IStaffService
    {

        private readonly IStaffRepository _staffRepository;
        //private readonly IBusinessRepository _businessRepository;
        public StaffService(IStaffRepository staffRepository /*IBusinessRepository businessRepository*/)
        {
            _staffRepository = staffRepository;
            // _businessRepository = businessRepository;
        }      
         public Staff CreateStaff(StaffRequest request, Guid id_Business)
         {

             var newStaff = request.ToStaff(id_Business);

            _staffRepository.Add(newStaff);
             return newStaff;
         }

        public Staff CreateStaffWhitBusiness(StaffRequest request)
        {
            var newBusiness = new Business
            {
                Id = Guid.NewGuid(),
                Name = $"{request.Name} - {request.BusinessCategory}",
                Url = $"http://www.{request.Name.Replace(" ", "")}.FCMTurniFy.com"
            };

            var newStaff = request.ToStaff(newBusiness);
                
            _staffRepository.Add(newStaff);

            return newStaff;
        }


        public void DeleteStaff(Guid id)
         {
            var User = _staffRepository.GetById(id) ?? throw new Exception("Usuario no encontrado");
            _staffRepository.Delete(id);
         }
         public List<Staff> GetAll()
         {
             return _staffRepository.GetAll();
         }

         public Staff? GetById(Guid id)
         {
            var existingStaff = _staffRepository.GetById(id) ?? throw new Exception("Usuario no encontrado");

            return existingStaff;
         }
        
         public Staff UpdateStaff(StaffRequest staff, Guid idStaff)
         {
            var existingStaff = _staffRepository.GetById(idStaff) ?? throw new Exception("Usuario no encontrado");

            existingStaff = staff.ToStaff(existingStaff.BusinessId) ?? throw new Exception();
            _staffRepository.Update(existingStaff);
                 return existingStaff;
        
         }
       
    }
}