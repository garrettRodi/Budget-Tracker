using System;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs
{
    public class PlannedExpenseDTO
    {
        public Guid Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public Money Amount { get; set; }
        public string Currency {  get; set; }
        public DateTime Period { get; set; }
    }
}
