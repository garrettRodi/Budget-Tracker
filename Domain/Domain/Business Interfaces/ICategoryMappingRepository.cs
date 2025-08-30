using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Domain.Interfaces
{
    public interface ICategoryMappingRepository
    {
        Task AddAsync(CategoryMapping mapping);
        Task<IEnumerable<CategoryMapping>> GetAllAsync();
        Task<CategoryMapping?> GetByCategoryNameAsync(string categoryName);
        Task<bool> UpdateAsync(CategoryMapping mapping);
        Task<bool> DeleteAsync(Guid id);
    }
}
