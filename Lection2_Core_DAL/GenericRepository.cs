using Lection2_Core_DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lection2_Core_DAL
{
    public class GenericRepository<T> where T : Entity, new()
    {
        private readonly EfDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(EfDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public async Task<T> CreateAsync(T entity)
        {
            entity.Id = Guid.NewGuid();
            _dbSet.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<T> GetByIdAsync(Guid id)
            => await _dbSet.FindAsync(id);

        public async Task<T> DeleteAsync(Guid id)
        {
            var entity = new T { Id = id };
            _dbContext.Entry(entity).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
