using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetTracker.Application.Services
{
    public class CategoryMappingService : ICategoryMappingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryMappingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddMappingAsync(string categoryName, string groupName)
        {
            var mapping = new CategoryMapping
            {
                Id = System.Guid.NewGuid(),
                CategoryName = categoryName,
                GroupName = groupName
            };
            await _unitOfWork.CategoryMappingRepository.AddAsync(mapping);
            await _unitOfWork.CommitAsync();
        }

        public async Task<string> GetGroupForCategoryAsync(string categoryName)
        {
            var mapping = await _unitOfWork.CategoryMappingRepository.GetByCategoryNameAsync(categoryName);
            return mapping?.GroupName ?? "Unmapped";
        }

        public async Task<IEnumerable<CategoryMappingDTO>> GetAllMappingsAsync()
        {
            var mappings = await _unitOfWork.CategoryMappingRepository.GetAllAsync();
            return mappings.Select(m => new CategoryMappingDTO
            {
                Id = m.Id,
                CategoryName = m.CategoryName,
                GroupName = m.GroupName
            });
        }
    }
}
