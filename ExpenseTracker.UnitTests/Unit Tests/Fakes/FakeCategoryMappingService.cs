using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.Interfaces;

namespace BudgetTracker.Tests.UnitTests.Fakes
{
    public class FakeCategoryMappingService : ICategoryMappingService
    {
        private readonly List<CategoryMappingDTO> _mappings = new List<CategoryMappingDTO>();

        public Task AddMappingAsync(string categoryName, string groupName)
        {
            _mappings.Add(new CategoryMappingDTO
            {
                Id = Guid.NewGuid(),
                CategoryName = categoryName,
                GroupName = groupName
            });
            return Task.CompletedTask;
        }

        public Task<IEnumerable<CategoryMappingDTO>> GetAllMappingsAsync()
        {
            return Task.FromResult<IEnumerable<CategoryMappingDTO>>(_mappings);
        }

        public Task<string> GetGroupForCategoryAsync(string categoryName)
        {
            var mapping = _mappings.Find(m => m.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(mapping != null ? mapping.GroupName : "Unmapped");
        }

        public Task<IEnumerable<string>> GetAllCategoryNamesAsync()
        {
            // Return the CategoryName of every mapping in our in-memory list
            var names = _mappings.Select(m => m.CategoryName);
            return Task.FromResult<IEnumerable<string>>(names);
        }
    }
}
