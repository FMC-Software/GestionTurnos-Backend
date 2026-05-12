using GestionTurnos.Application.Abstraction;
using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using System.Xml.Linq;

namespace GestionTurnos.Application.Services
{
    public class BusinessService : IBusinessService
    {
        private readonly IBusinessRepository _businessRepository;
        public BusinessService(IBusinessRepository businessRepository)
        {
            _businessRepository = businessRepository;
        }
        public Business Create(Business business)
        {
            _businessRepository.Add(business);
            return business;
        }

        public void Delete(Guid id)
        {

            _businessRepository.Delete(id);
             
        }

        public List<Business> GetAll()
        {
            return _businessRepository.GetAll();
        }

        public Business GetById(Guid id)
        {

            return _businessRepository.GetById(id);
        }

        public void Update(Business value)
        {
            var existingClient = _businessRepository.GetById(value.Id) ?? throw new Exception("Empresa no encontrada");
            _businessRepository.Update(value);
        }
    }
}