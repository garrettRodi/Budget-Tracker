using BudgetTracker.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetTracker.Application.Interfaces
{
    public interface ICategoryMappingService
    {
        Task AddMappingAsync(string categoryName, string groupName);
        Task<string> GetGroupForCategoryAsync(string categoryName);
        Task<IEnumerable<CategoryMappingDTO>> GetAllMappingsAsync();
        Task<IEnumerable<string>> GetAllCategoryNamesAsync();
    }
}
