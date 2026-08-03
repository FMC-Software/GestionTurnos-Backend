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
        public virtual async Task<T> Add(T entity)
        {
            _dbSet.Add(entity);
            await SaveChanges();
            return entity;
        }

        public virtual async Task Delete(Guid id)
        {
            var EntityUpdate = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
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
                await SaveChanges();
            }

        }

        public virtual async Task<List<T>> GetAllGlobal()
        {
            return await _dbSet.Where(x => !x.IsDeleted).ToListAsync();
        }

        public virtual async Task<T?> GetById(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(x=>x.Id == id && !x.IsDeleted);
        }

        public virtual async Task Update(T entity)
        {
            entity.UpdateDateTime = DateTime.UtcNow;
            _dbSet.Update(entity);
            await SaveChanges();
        }

        protected async Task SaveChanges()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseException("Error al acceder a la base de datos.", ex);
            }
        }
    }
}
