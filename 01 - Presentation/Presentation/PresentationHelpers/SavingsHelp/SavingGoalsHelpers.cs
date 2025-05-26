// File: Presentation/SavingGoalsHelpers/SavingGoalsHelpers.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.PresentationHelpers
{
    public class SavingGoalsHelpers
    {
        private readonly ISavingGoalsService _savingGoalsService;
        private readonly SelectBudgetContainer _selector;
        private readonly InputProcessor _input;
        private readonly IConsole _console;

        public SavingGoalsHelpers(
            ISavingGoalsService savingGoalsService,
            SelectBudgetContainer selector,
            InputProcessor input,
            IConsole console)
        {
            _savingGoalsService = savingGoalsService
                ?? throw new ArgumentNullException(nameof(savingGoalsService));
            _selector = selector
                ?? throw new ArgumentNullException(nameof(selector));
            _input = input
                ?? throw new ArgumentNullException(nameof(input));
            _console = console
                ?? throw new ArgumentNullException(nameof(console));
        }

        public async Task CreateSavingGoalAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Create Saving Goal ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var goalName = _input.GetInput("Enter saving goal name: ");
            var targetAmount = _input.GetValidDecimal("Enter target amount: ");
            var currentAmount = _input.GetValidDecimal("Enter current amount: ");
            var targetDate = _input.GetValidDate("Enter target date (yyyy-MM-dd): ");

            var cmd = new CreateSavingGoalCommand
            {
                BudgetContainerId = budgetId,
                GoalName = goalName,
                TargetAmount = targetAmount,
                CurrentAmount = currentAmount,
                TargetDate = targetDate
            };

            var dto = await _savingGoalsService.CreateSavingGoalAsync(cmd);
            _console.WriteLine($"Saving goal '{dto.GoalName}' created with ID: {dto.Id}");
            _console.ReadKey();
        }

        public async Task ViewSavingGoalsAsync()
        {
            _console.Clear();
            _console.WriteLine("=== View Saving Goals ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var list = await _savingGoalsService.GetSavingGoalsByBudgetContainerIdAsync(budgetId);
            foreach (var goal in list)
            {
                _console.WriteLine(
                    $"ID: {goal.Id} | Name: {goal.GoalName} | Target: {goal.TargetAmount:C} | " +
                    $"Current: {goal.CurrentAmount:C} | Target Date: {goal.TargetDate:yyyy-MM-dd}");
            }
            if (!list.Any())
                _console.WriteLine("No saving goals found for the active budget.");
            _console.ReadKey();
        }

        public async Task UpdateSavingGoalAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Update Saving Goal ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var idInput = _input.GetInput("Enter saving goal ID to update: ");
            if (!Guid.TryParse(idInput, out var id))
            {
                _console.WriteLine("Invalid ID format.");
                return;
            }

            var goalName = _input.GetInput("Enter updated goal name: ");
            var targetAmount = _input.GetValidDecimal("Enter updated target amount: ");
            var currentAmount = _input.GetValidDecimal("Enter updated current amount: ");
            var targetDate = _input.GetValidDate("Enter updated target date (yyyy-MM-dd): ");

            var cmd = new UpdateSavingGoalCommand
            {
                BudgetContainerId = budgetId,
                Id = id,
                GoalName = goalName,
                TargetAmount = targetAmount,
                CurrentAmount = currentAmount,
                TargetDate = targetDate
            };

            bool success = await _savingGoalsService.UpdateSavingGoalAsync(cmd);
            _console.WriteLine(success
                ? "Saving goal updated successfully."
                : "Saving goal update failed.");
            _console.ReadKey();
        }

        public async Task DeleteSavingGoalAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Delete Saving Goal ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var idInput = _input.GetInput("Enter saving goal ID to delete: ");
            if (!Guid.TryParse(idInput, out var id))
            {
                _console.WriteLine("Invalid ID format.");
                return;
            }

            bool success = await _savingGoalsService.DeleteSavingGoalAsync(id);
            _console.WriteLine(success
                ? "Saving goal deleted successfully."
                : "Saving goal deletion failed.");
            _console.ReadKey();
        }
    }
}
