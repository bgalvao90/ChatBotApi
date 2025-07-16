using System.Linq.Expressions;
using ChatBotApi.Context;
using ChatBotApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatBotApi.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public Task<T> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return Task.FromResult(entity);
        }

        public Task<T> DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return Task.FromResult(entity);
        }
    }
}
