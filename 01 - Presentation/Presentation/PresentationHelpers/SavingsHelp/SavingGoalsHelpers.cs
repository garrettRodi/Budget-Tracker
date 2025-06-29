// File: Presentation/SavingGoalsHelpers/SavingGoalsHelpers.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Services;
using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.PresentationHelpers
{
    public class SavingGoalsHelpers
    {
        private readonly ISavingGoalsService _savingGoalsService;
        private readonly SelectBudgetContainer _selector;
        private readonly InputProcessor _input;
        private readonly IConsole _console;
        private readonly ICurrencyService _currencyService;

        public SavingGoalsHelpers(
            ISavingGoalsService savingGoalsService,
            SelectBudgetContainer selector,
            InputProcessor input,
            IConsole console,
            ICurrencyService currencyService)
        {
            _savingGoalsService = savingGoalsService
                ?? throw new ArgumentNullException(nameof(savingGoalsService));
            _selector = selector
                ?? throw new ArgumentNullException(nameof(selector));
            _input = input
                ?? throw new ArgumentNullException(nameof(input));
            _console = console
                ?? throw new ArgumentNullException(nameof(console));
            _currencyService = currencyService;
        }

        public async Task CreateSavingGoalAsync()
        {
            bool create;
            do
            {
                try
                {
                    _console.Clear();
                    _console.WriteLine("=== Create Saving Goal ===");

                    var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
                    if (budgetId == Guid.Empty)
                    {
                        _console.WriteLine("No active budget found. Please create or select a budget first.");
                        _console.ReadKey();
                        return;
                    }

                    var goalName = _input.GetTitleInput("Enter saving goal name: ");
                    var targetAmount = _input.GetValidDecimal("Enter target amount: ");
                    var currentAmount = _input.GetValidDecimal("Enter current amount: ");
                    var targetDate = _input.GetValidDate("Enter target date (yyyy-MM-dd): ", allowFuture: true);

                    var cmd = new CreateSavingGoalCommand
                    {
                        BudgetContainerId = budgetId,
                        GoalName = goalName,
                        TargetAmount = new Money(targetAmount, _currencyService.CurrentCurrency),
                        CurrentAmount = new Money(currentAmount, _currencyService.CurrentCurrency),
                        TargetDate = targetDate
                    };

                    var dto = await _savingGoalsService.CreateSavingGoalAsync(cmd);
                    _console.WriteLine($"Saving goal '{dto.GoalName}' created with ID: {dto.Id}");
                    _console.ReadKey();
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"Error creating saving goal: {ex.Message}");
                    _console.ReadKey();
                }
                create = _input.GetBool("Do you want to create another saving goal? (y/n): ");
            } while (create);
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
                    $"ID: {goal.Id} | Name: {goal.GoalName} | Target: {goal.TargetAmount.ToDisplay(_currencyService)} | " +
                    $"Current: {goal.CurrentAmount.ToDisplay(_currencyService)} | Target Date: {goal.TargetDate:yyyy-MM-dd}");
            }
            if (!list.Any())
                _console.WriteLine("No saving goals found for the active budget.");
            _console.ReadKey();
        }

        public async Task UpdateSavingGoalAsync()
        {
            bool update;
            do
            {

                try
                {
                    _console.Clear();
                    _console.WriteLine("=== Update Saving Goal ===");

                    var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
                    if (budgetId == Guid.Empty)
                    {
                        _console.WriteLine("No active budget found. Please create or select a budget first.");
                        _console.ReadKey();
                        return;
                    }

                    var list = (await _savingGoalsService.GetSavingGoalsByBudgetContainerIdAsync(budgetId)).ToList();

                    if (!list.Any())
                    {
                        _console.WriteLine("No saving goals found for the active budget.");
                        _console.ReadKey();
                        return;
                    }

                    foreach (var goal in list)
                    {
                        _console.WriteLine(
                            $"ID: {goal.Id} | Name: {goal.GoalName} | Target: {goal.TargetAmount.ToDisplay(_currencyService)} | " +
                            $"Current: {goal.CurrentAmount.ToDisplay(_currencyService)} | Target Date: {goal.TargetDate:yyyy-MM-dd}");
                    }

                    Guid id;

                    while (true)
                    {
                        id = _input.GetValidGuid("Enter the ID of the saving goal to update.");
                        if (list.Any(x => x.Id == id)) break;
                        _console.WriteLine("Invalid ID. Please choose from the list above.");
                    }

                    var existing = list.First(x => x.Id == id);

                    string goalName = _input.GetTitleInput($"Name ({existing.GoalName}): ");
                    decimal targetAmount = _input.GetValidDecimal($"Target Amount ({existing.TargetAmount:C}): ");
                    decimal currentAmount = _input.GetValidDecimal($"Current Amount ({existing.CurrentAmount:C}): ");
                    DateTime targetDate = _input.GetValidDate($"Target Date ({existing.TargetDate:yyyy-MM-dd}): ", allowFuture: true);

                    var cmd = new UpdateSavingGoalCommand
                    {
                        BudgetContainerId = budgetId,
                        Id = id,
                        GoalName = goalName,
                        TargetAmount = new Money(targetAmount, _currencyService.CurrentCurrency),
                        CurrentAmount = new Money(currentAmount, _currencyService.CurrentCurrency),
                        TargetDate = targetDate
                    };

                    bool success = await _savingGoalsService.UpdateSavingGoalAsync(cmd);
                    _console.WriteLine(success
                        ? "Saving goal updated successfully."
                        : "Saving goal update failed.");
                    _console.ReadKey();
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"Error updating saving goal: {ex.Message}");
                    _console.ReadKey();
                }
                update = _input.GetBool("Do you want to update another saving goal? (y/n): ");
            } while (update);
        }

        public async Task DeleteSavingGoalAsync()
        {
            bool delete;
            do
            {
                try
                {

                    _console.Clear();
                    _console.WriteLine("=== Delete Saving Goal ===");

                    var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
                    if (budgetId == Guid.Empty)
                    {
                        _console.WriteLine("No active budget found.");
                        _console.ReadKey();
                        return;
                    }

                    var list = (await _savingGoalsService.GetSavingGoalsByBudgetContainerIdAsync(budgetId)).ToList();

                    if (!list.Any())
                    {
                        _console.WriteLine("No saving goals to delete.");
                        _console.ReadKey();
                        return;
                    }

                    foreach (var g in list)
                    {
                        _console.WriteLine(
                            $"ID: {g.Id} | Name: {g.GoalName} | Target: {g.TargetAmount:C} | " +
                            $"Current: {g.CurrentAmount:C} | Target Date: {g.TargetDate:yyyy-MM-dd}");
                    }

                    Guid id;
                    while (true)
                    {
                        id = _input.GetValidGuid("Enter the ID of the saving goal to delete.");
                        if (list.Any(x => x.Id == id)) break;
                        _console.WriteLine("Invalid ID. Please choose from the list above.");
                    }

                    bool success = await _savingGoalsService.DeleteSavingGoalAsync(id);
                    _console.WriteLine(success
                        ? "Saving goal deleted successfully."
                        : "Saving goal deletion failed.");
                    _console.ReadKey();
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"Error deleting saving goal: {ex.Message}");
                    _console.ReadKey();
                }
                delete = _input.GetBool("Do you want to delete another saving goal? (y/n): ");
            } while (delete);
        }
    }
}
