using System;

namespace BudgetTracker.Application.DTOs
{
    public class CategoryMappingDTO
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
    }
}
