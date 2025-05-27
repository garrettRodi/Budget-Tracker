using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetTracker.Tests.UnitTests.Fakes
{
    public class FakeSavingGoalsService : ISavingGoalsService
    {
        private readonly List<SavingGoalDTO> _goals = new List<SavingGoalDTO>();

        public Task<SavingGoalDTO> CreateSavingGoalAsync(CreateSavingGoalCommand command)
        {
            var goal = new SavingGoalDTO
            {
                Id = Guid.NewGuid(),
                GoalName = command.GoalName,
                TargetAmount = command.TargetAmount,
                CurrentAmount = command.CurrentAmount,
                TargetDate = command.TargetDate,
                BudgetContainerId = command.BudgetContainerId
            };
            _goals.Add(goal);
            return Task.FromResult(goal);
        }

        public Task<bool> DeleteSavingGoalAsync(Guid id)
        {
            var goal = _goals.FirstOrDefault(g => g.Id == id);
            if (goal != null)
            {
                _goals.Remove(goal);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<IEnumerable<SavingGoalDTO>> GetAllSavingGoalsAsync()
        {
            return Task.FromResult<IEnumerable<SavingGoalDTO>>(_goals);
        }

        public Task<IEnumerable<SavingGoalDTO>> GetSavingGoalsByBudgetContainerIdAsync(Guid budgetContainerId)
        {
            // Ensure your DTO has a BudgetContainerId property
            var filteredGoals = _goals.Where(g => g.BudgetContainerId == budgetContainerId);
            return Task.FromResult(filteredGoals);
        }

        public Task<SavingGoalDTO?> GetSavingGoalByIdAsync(Guid id)
        {
            var goal = _goals.FirstOrDefault(g => g.Id == id);
            return Task.FromResult<SavingGoalDTO?>(goal);
        }

        public Task<bool> UpdateSavingGoalAsync(UpdateSavingGoalCommand command)
        {
            var goal = _goals.FirstOrDefault(g => g.Id == command.Id);
            if (goal != null)
            {
                goal.GoalName = command.GoalName;
                goal.TargetAmount = command.TargetAmount;
                goal.CurrentAmount = command.CurrentAmount;
                goal.TargetDate = command.TargetDate;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        public Task RecalculateCurrentAmountAsync(Guid savingGoalId)
        {
            // No real recalculation needed in fake — just simulate success
            return Task.CompletedTask;
        }

    }
}
