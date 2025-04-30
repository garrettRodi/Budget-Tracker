using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Infrastructure.RepositoryImplementations
{

    public class CategoryMappingRepository
         : GenericRepository<CategoryMapping>, ICategoryMappingRepository
    {
        public CategoryMappingRepository(BudgetTrackerDbContext context, ILogger<CategoryMappingRepository> logger)
            : base(context)
        {
        }

        public async Task<CategoryMapping?> GetByCategoryNameAsync(string categoryName)
        {
            return await _dbSet
                .FirstOrDefaultAsync(m =>
                    m.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
        }
    }
}

