using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.Mappers
{
    public static class SavingGoalMapper
    {
        public static SavingGoals ToEntity(this CreateSavingGoalCommand command)
        {
            return new SavingGoals
            {
                Id = Guid.NewGuid(),
                GoalName = command.GoalName,
                TargetAmount = command.TargetAmount,
                CurrentAmount = command.CurrentAmount,
                TargetDate = command.TargetDate,
                BudgetContainerId = command.BudgetContainerId
            };
        }

        public static SavingGoalDTO ToDto(this SavingGoals goal)
        {
            return new SavingGoalDTO
            {
                Id = goal.Id,
                GoalName = goal.GoalName,
                TargetAmount = goal.TargetAmount,
                CurrentAmount = goal.CurrentAmount,
                TargetDate = goal.TargetDate
            };
        }
    }
}
