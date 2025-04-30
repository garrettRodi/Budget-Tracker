using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Infrastructure.RepositoryImplementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly BudgetTrackerDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(BudgetTrackerDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            if (_dbSet == null)
            {
                throw new InvalidOperationException($"DbSet for type {typeof(T).Name} is null. Ensure the entity is part of the model.");
            }
        }


        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Add(entity);
            await Task.CompletedTask;
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Update(entity);
            await Task.CompletedTask;
            return true;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;
            _dbSet.Remove(entity);
            await Task.CompletedTask;
            return true;
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}
