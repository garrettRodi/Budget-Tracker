using System;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs.Commands
{
    public class CreatePlannedExpenseCommand
    {
        public Guid BudgetContainerId { get; set; }
        public string Category { get; set; } = string.Empty;
        public Money Amount { get; set; }
        public DateTime Period { get; set; }
    }
}
