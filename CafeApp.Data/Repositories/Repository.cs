using CafeApp.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CafeApp.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            // Detach any existing tracked entities with the same key to avoid conflicts
            var entry = _context.Entry(entity);

            // If the entity is already tracked, just mark it as modified
            if (entry.State != EntityState.Detached)
            {
                entry.State = EntityState.Modified;
                return entity;
            }

            // Check for any other tracked entities with the same key and detach them
            var entityType = entity.GetType();
            var keyProperty = entityType.GetProperty("Id");

            if (keyProperty != null)
            {
                var keyValue = keyProperty.GetValue(entity);
                if (keyValue != null)
                {
                    var trackedEntities = _context.ChangeTracker.Entries()
                        .Where(e => e.Entity.GetType() == entityType)
                        .Where(e => keyProperty.GetValue(e.Entity)?.Equals(keyValue) == true)
                        .ToList();

                    foreach (var trackedEntity in trackedEntities)
                    {
                        trackedEntity.State = EntityState.Detached;
                    }
                }
            }

            // Now safely update the entity
            _dbSet.Update(entity);
            return entity;
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }
    }
}
