using System;

namespace BudgetTracker.Application.DTOs
{
    public class SavingGoalDTO
    {
        public Guid Id { get; set; }
        public string GoalName { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime? TargetDate { get; set; }
        public Guid BudgetContainerId { get; set; }
    }
}
