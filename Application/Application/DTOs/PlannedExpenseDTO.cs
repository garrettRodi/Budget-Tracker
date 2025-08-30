using System;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs
{
    public class PlannedExpenseDTO
    {
        public Guid Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Name { get; set; } = null!;
        public Money Amount { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public DateTime Period { get; set; }
    }
}
