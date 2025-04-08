using System;

namespace BudgetTracker.Application.DTOs.Commands
{
    public class CreateSavingGoalCommand
    {
        public Guid BudgetContainerId { get; set; }

        public string GoalName { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime? TargetDate { get; set; }
    }
}
