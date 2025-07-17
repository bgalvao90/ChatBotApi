using System.Linq.Expressions;

namespace ChatBotApi.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
    }
}
