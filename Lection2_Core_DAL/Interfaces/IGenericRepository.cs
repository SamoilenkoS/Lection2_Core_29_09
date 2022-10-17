using Lection2_Core_DAL.Entities;
using System.Linq.Expressions;

namespace Lection2_Core_DAL.Interfaces
{
    public interface IGenericRepository<T> where T : Entity, new()
    {
        Task<T> CreateAsync(T entity);
        Task<T> GetByIdAsync(Guid id);
        Task<T> DeleteAsync(Guid id);
        Task<T> UpdateAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetByPredicateAsync(Expression<Func<T, bool>> expression);
    }
}
