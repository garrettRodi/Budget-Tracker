using System;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs.Commands
{
    public class UpdatePlannedExpenseCommand
    {
        public Guid Id { get; set; }
        public Guid BudgetContainerId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Name { get; set; } = null!;
        public Money Amount { get; set; } = null!;
        public DateTime Period { get; set; }
    }
}
