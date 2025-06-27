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
    public class PlannedIncomeHelpers
    {
        private readonly IPlannedIncomeService _plannedService;
        private readonly SelectBudgetContainer _selector;
        private readonly InputProcessor _input;
        private readonly IConsole _console;
        private readonly ICurrencyService _currencyService;

        public PlannedIncomeHelpers(
            IPlannedIncomeService plannedService,
            SelectBudgetContainer selector,
            InputProcessor input,
            IConsole console,
            ICurrencyService currencyService)
        {
            _plannedService = plannedService;
            _selector = selector;
            _input = input;
            _console = console;
            _currencyService = currencyService;
        }

        public async Task CreatePlannedIncomeAsync()
        {
            bool create;
            do
            {
                try
                {
                    _console.Clear();
                    _console.WriteLine("=== Create Planned Income ===");

                    Guid budgetId = await _selector.GetActiveBudgetContainerIdAsync();
                    if (budgetId == Guid.Empty) return;

                    DateTime period = _input.GetValidDate("Enter period date (yyyy-MM-dd): ", allowFuture: true);
                    decimal amount = _input.GetValidDecimal("Enter planned amount: ");
                    string source = _input.GetTitleInput("Enter income source: ");

                    var cmd = new CreatePlannedIncomeCommand
                    {
                        BudgetContainerId = budgetId,
                        Source = source,
                        PeriodStart = period,
                        Amount = new Money(amount, _currencyService.CurrentCurrency)
                    };

                    var dto = await _plannedService.CreatePlannedIncomeAsync(cmd);
                    _console.WriteLine($"Planned income for {dto.Source} created (ID: {dto.Id})");
                    _console.ReadKey();
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"Error creating planned income: {ex.Message}");
                    _console.ReadKey();
                }
                create = _input.GetBool("Do you want to create another planned income? (y/n): ");
            } while (create);
        }

        public async Task ViewPlannedIncomesAsync()
        {
            _console.Clear();
            _console.WriteLine("=== View Planned Incomes ===");

            Guid budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var list = await _plannedService.GetPlannedIncomesByBudgetAsync(budgetId);
            foreach (var pi in list)
            {
                _console.WriteLine($"ID: {pi.Id} |Source: {pi.Source} | Date: {pi.PeriodStart:d} | Amount: {pi.Amount:C}");
            }
            if (!list.Any())
                _console.WriteLine("No planned incomes found.");
            _console.ReadKey();
        }

        public async Task UpdatePlannedIncomeAsync()
        {
            bool update;
            do
            {
                try
                {
                    _console.Clear();
                    _console.WriteLine("=== Update Planned Income ===");

                    Guid budgetId = await _selector.GetActiveBudgetContainerIdAsync();
                    if (budgetId == Guid.Empty)
                    {
                        _console.WriteLine("No active budget found.");
                        _console.ReadKey();
                        return;
                    }
                    var list = await _plannedService.GetPlannedIncomesByBudgetAsync(budgetId);
                    if (!list.Any())
                    {
                        _console.WriteLine("No planned incomes available to update.");
                        _console.ReadKey();
                        return;
                    }

                    foreach (var pi in list)
                    {
                        _console.WriteLine($"ID: {pi.Id} | Source: {pi.Source} | Date: {pi.PeriodStart:d} | Amount: {pi.Amount:C}");
                    }

                    Guid id;
                    while (true)
                    {
                        _console.Write("Enter planned income ID to update: ");
                        var input = _console.ReadLine();
                        if (Guid.TryParse(input, out id) && list.Any(pi => pi.Id == id))
                            break;
                        _console.WriteLine("Invalid ID. Please try again.");
                    }

                    var existing = list.First(pi => pi.Id == id);

                    string source = _input.GetTitleInput($"Source ({existing.Source}): ");
                    DateTime period = _input.GetValidDate($"Date ({existing.PeriodStart:yyyy-MM-dd}): ", allowFuture: true);
                    decimal amount = _input.GetValidDecimal($"Amount ({existing.Amount:C}): ");

                    var cmd = new UpdatePlannedIncomeCommand
                    {
                        Id = id,
                        Source = source,
                        PeriodStart = period,
                        Amount = new Money(amount, _currencyService.CurrentCurrency)
                    };

                    bool ok = await _plannedService.UpdatePlannedIncomeAsync(cmd);
                    _console.WriteLine(ok
                        ? "Updated successfully."
                        : "Update failed.");
                    _console.ReadKey();
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"Error updating planned income: {ex.Message}");
                    _console.ReadKey();
                }
                update = _input.GetBool("Do you want to update another planned income? (y/n): ");
            } while (update);
        }

        public async Task DeletePlannedIncomeAsync()
        {
            bool delete;
            do
            {
                try
                {
                    _console.Clear();
                    _console.WriteLine("=== Delete Planned Income ===");

                    Guid budgetId = await _selector.GetActiveBudgetContainerIdAsync();
                    if (budgetId == Guid.Empty)
                    {
                        _console.WriteLine("No active budget found.");
                        _console.ReadKey();
                        return;
                    }

                    var list = await _plannedService.GetPlannedIncomesByBudgetAsync(budgetId);
                    if (!list.Any())
                    {
                        _console.WriteLine("No planned incomes available to delete.");
                        _console.ReadKey();
                        return;
                    }

                    foreach (var pi in list)
                    {
                        _console.WriteLine($"ID: {pi.Id} | Source: {pi.Source} | Date: {pi.PeriodStart:d} | Amount: {pi.Amount:C}");
                    }

                    Guid id;
                    while (true)
                    {
                        _console.Write("Enter planned income ID to delete: ");
                        var input = _console.ReadLine();
                        if (Guid.TryParse(input, out id) && list.Any(pi => pi.Id == id))
                            break;
                        _console.WriteLine("Invalid ID. Please try again.");
                    }

                    bool ok = await _plannedService.DeletePlannedIncomeAsync(id);
                    _console.WriteLine(ok
                        ? "Deleted successfully."
                        : "Delete failed.");
                    _console.ReadKey();
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"Error deleting planned income: {ex.Message}");
                    _console.ReadKey();
                }
                delete = _input.GetBool("Do you want to delete another planned income? (y/n): ");
            } while (delete);
        }
    }
}
