using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Abstraction
{
    public interface IBusinessService
    {

        List<Business> GetAll();
        Business GetById(Guid id);

        Business Create(Business business);

        void Update(Business value);

        void Delete (Guid id);

        public List<Staff> GetAllByBusiness(Guid id_Business);
       
    }
}
