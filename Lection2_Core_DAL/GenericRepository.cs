using Lection2_Core_DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lection2_Core_DAL
{
    public class GenericRepository<T> where T : Entity
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
    }
}
