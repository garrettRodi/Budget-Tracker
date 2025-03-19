using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BudgetTracker.Tests.UnitTests.Fakes
{
    public class FakeCategoryMappingRepository : ICategoryMappingRepository
    {
        private readonly List<CategoryMapping> _mappings = new List<CategoryMapping>();

        public Task AddAsync(CategoryMapping mapping)
        {
            _mappings.Add(mapping);
            return Task.CompletedTask;
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var mapping = _mappings.FirstOrDefault(m => m.Id == id);
            if (mapping != null)
            {
                _mappings.Remove(mapping);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<IEnumerable<CategoryMapping>> FindAsync(Expression<Func<CategoryMapping, bool>> predicate)
        {
            var result = _mappings.AsQueryable().Where(predicate).ToList();
            return Task.FromResult<IEnumerable<CategoryMapping>>(result);
        }

        public Task<IEnumerable<CategoryMapping>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<CategoryMapping>>(_mappings);
        }

        public Task<CategoryMapping?> GetByCategoryNameAsync(string categoryName)
        {
            var mapping = _mappings.FirstOrDefault(m => m.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(mapping);
        }

        public Task<CategoryMapping?> GetByIdAsync(Guid id)
        {
            var mapping = _mappings.FirstOrDefault(m => m.Id == id);
            return Task.FromResult(mapping);
        }

        public Task<bool> UpdateAsync(CategoryMapping mapping)
        {
            var index = _mappings.FindIndex(m => m.Id == mapping.Id);
            if (index >= 0)
            {
                _mappings[index] = mapping;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
