using System;

namespace BudgetTracker.Application.DTOs
{
    public class PlannedExpenseDTO
    {
        public Guid Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Period { get; set; }
    }
}
