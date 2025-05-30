using System;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Domain.Entities
{
    public class PlannedExpense
    {
        public Guid Id { get; set; }
        public SavingGoals? SavingGoal { get; set; }
        public Guid BudgetContainerId { get; set; }
        public BudgetContainer? BudgetContainer { get; set; }

        public string Category { get; set; } = string.Empty;
        public Money Amount { get; set; }

        // The day (for weekly/monthly budgets) or month‐start (for yearly budgets)
        public DateTime Period { get; set; }
    }
}
