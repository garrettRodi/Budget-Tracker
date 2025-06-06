using System;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs.Commands
{
    public class CreateSavingGoalCommand
    {
        public Guid BudgetContainerId { get; set; }

        public string GoalName { get; set; } = string.Empty;
        public Money TargetAmount { get; set; }
        public Money CurrentAmount { get; set; }
        public DateTime? TargetDate { get; set; }
    }
}
