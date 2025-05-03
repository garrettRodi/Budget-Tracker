using System.Text.RegularExpressions;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using Microsoft.Extensions.Logging;


namespace BudgetTracker.Application.Services
{
    public class CategoryMappingService : ICategoryMappingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryMappingService> _logger;

        public CategoryMappingService(IUnitOfWork unitOfWork, ILogger<CategoryMappingService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task AddMappingAsync(string categoryName, string groupName)
        {
            _logger.LogInformation("Adding mapping for category '{CategoryName}' with group '{GroupName}'.", categoryName, groupName);

            var mapping = new CategoryMapping
            {
                Id = System.Guid.NewGuid(),
                CategoryName = categoryName,
                GroupName = groupName
            };
            await _unitOfWork.CategoryMappingRepository.AddAsync(mapping);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Successfully added mapping with ID '{MappingId}'.", mapping.Id);
        }

        public async Task<string> GetGroupForCategoryAsync(string categoryName)
        {
            _logger.LogInformation("Retrieving group for category '{CategoryName}'.", categoryName);
            
            var mapping = await _unitOfWork.CategoryMappingRepository.GetByCategoryNameAsync(categoryName);
            string group = mapping?.GroupName ?? "Unmapped";
            
            _logger.LogInformation("Retrieved group: '{Group}'.", group);
            return group;
        }

        public async Task<IEnumerable<CategoryMappingDTO>> GetAllMappingsAsync()
        {
            _logger.LogInformation("Retrieving all category mappings.");
            var mappings = await _unitOfWork.CategoryMappingRepository.GetAllAsync();
            var dtos = mappings.Select(m => new CategoryMappingDTO
            {
                Id = m.Id,
                CategoryName = m.CategoryName,
                GroupName = m.GroupName
            }).ToList();

            _logger.LogInformation("Retrieved {Count} mappings.", dtos.Count);
            return dtos;
        }

        public async Task<IEnumerable<string>> GetAllCategoryNamesAsync()
        {
            var mappings = await _unitOfWork.CategoryMappingRepository.GetAllAsync();
            return mappings.Select(m => m.CategoryName);
        }
    }
}
