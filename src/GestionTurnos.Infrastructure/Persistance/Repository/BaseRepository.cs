using GestionTurnos.Application.Abstraction.Infrastructure;
using GestionTurnos.Application.Exceptions;
using GestionTurnos.Domain.Entities;
using GestionTurnos.Infrastructure.Persistence;
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
            SaveChanges();
            return entity;
        }

        public virtual void Delete(Guid id)
        {
            var EntityUpdate = _dbSet.FirstOrDefault(x => x.Id == id);
            if (EntityUpdate.IsDeleted == true)
            {
                throw new ConflictException("El registro ya se encuentra eliminado.");
            }
            if (EntityUpdate != null)
            {
                EntityUpdate.IsDeleted = true;
                EntityUpdate.DeleteDateTime = DateTime.UtcNow;
                EntityUpdate.UpdateDateTime = DateTime.UtcNow;
                _dbSet.Update(EntityUpdate);
                SaveChanges();
            }
           
        }

        public virtual List<T> GetAllGlobal()
        {
            return _dbSet.Where(x => !x.IsDeleted).ToList();
        }

        public virtual T? GetById(Guid id)
        {
            return _dbSet.FirstOrDefault(x=>x.Id == id);
        }

        public virtual void Update(T entity)
        {
            entity.UpdateDateTime = DateTime.UtcNow;
            _dbSet.Update(entity);
            SaveChanges();
        }

        protected void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseException("Error al acceder a la base de datos.", ex);
            }
        }
    }
}
