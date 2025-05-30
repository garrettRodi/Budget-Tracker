using System;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs
{
    public class SavingGoalDTO
    {
        public Guid Id { get; set; }
        public string GoalName { get; set; } = string.Empty;
        public Money TargetAmount { get; set; }
        public Money CurrentAmount { get; set; }
        public string Currency {  get; set; }
        public DateTime? TargetDate { get; set; }
        public Guid BudgetContainerId { get; set; }
    }
}
