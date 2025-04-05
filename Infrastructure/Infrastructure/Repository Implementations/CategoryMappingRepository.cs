using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Infrastructure.RepositoryImplementations
{
    public class CategoryMappingRepository : ICategoryMappingRepository
    {
        private readonly BudgetTrackerDbContext _context;
        ILogger<CategoryMappingRepository> _logger;

        public CategoryMappingRepository(BudgetTrackerDbContext context, ILogger<CategoryMappingRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(CategoryMapping mapping)
        {
            await _context.CategoryMappings.AddAsync(mapping);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryMapping>> GetAllAsync()
        {
            return await _context.CategoryMappings.ToListAsync();
        }

        public async Task<CategoryMapping?> GetByCategoryNameAsync(string categoryName)
        {
            return await _context.CategoryMappings.FirstOrDefaultAsync(m => m.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> UpdateAsync(CategoryMapping mapping)
        {
            _context.CategoryMappings.Update(mapping);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var mapping = await _context.CategoryMappings.FindAsync(id);
            if (mapping == null)
                return false;
            _context.CategoryMappings.Remove(mapping);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
