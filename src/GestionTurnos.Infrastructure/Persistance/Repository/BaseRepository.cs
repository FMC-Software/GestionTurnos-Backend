using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionTurnos.Infrastructure.Persistance.Repository
{
    
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly FMCTurnosDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(FMCTurnosDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public virtual T Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public virtual void Delete(Guid id)
        {
            var EntityUpdate = _dbSet.FirstOrDefault(x => x.Id == id);
            if (EntityUpdate != null)
            {
                EntityUpdate?.IsDeleted = true;
                _dbSet.Update(EntityUpdate);
                _context.SaveChanges();
            }
           
        }

        public virtual List<T> GetAll()
        {
            return _dbSet.Where(x => !x.IsDeleted).ToList();
        }

        public virtual T? GetById(Guid id)
        {
            return _dbSet.FirstOrDefault(x=>x.Id == id);
        }

        public virtual void Update(T entity)
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
        }
    }
}
