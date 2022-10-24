using Lection2_Core_DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lection2_Core_DAL
{
    public class BasicGenericRepository<T> : IBasicGenericRepository<T> where T : class, new()
    {
        protected readonly EfDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public BasicGenericRepository(EfDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            _dbSet.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public IQueryable<T> GetByPredicate(Expression<Func<T, bool>> expression)
            => _dbSet.Where(expression);

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<bool> DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return await _dbContext.SaveChangesAsync() != 0;
        }
    }
}
