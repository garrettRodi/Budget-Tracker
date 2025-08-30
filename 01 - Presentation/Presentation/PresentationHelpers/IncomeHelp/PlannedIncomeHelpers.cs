using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Services;
using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Presentation.UIHelpers;
using BudgetTracker.Presentation.PresentationHelpers;

namespace BudgetTracker.Presentation.PresentationHelpers
{
    public class PlannedIncomeHelpers
    {
        private readonly IPlannedIncomeService _plannedIncomeService;
        private readonly IBudgetService _budgetService;
        private readonly SelectBudgetContainer _selector;
        private readonly InputProcessor _input;
        private readonly IConsole _console;
        private readonly ICurrencyService _currencyService;

        public PlannedIncomeHelpers(
            IPlannedIncomeService plannedIncomeService,
            IBudgetService budgetService,
            SelectBudgetContainer selector,
            InputProcessor input,
            IConsole console,
            ICurrencyService currencyService)
        {
            _plannedIncomeService = plannedIncomeService;
            _budgetService = budgetService;
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

                    var budget = await _budgetService.GetBudgetByIdAsync(budgetId);
                    string nativeCurrency = budget.Currency;

                    DateTime period = _input.GetValidDate("Enter period date (yyyy-MM-dd): ", allowFuture: true);
                    decimal amount = _input.GetValidDecimal("Enter planned amount: ");
                    string source = _input.GetTitleInput("Enter income source: ");

                    var cmd = new CreatePlannedIncomeCommand
                    {
                        BudgetContainerId = budgetId,
                        Source = source,
                        PeriodStart = period,
                        Amount = new Money(amount, nativeCurrency)
                    };

                    var dto = await _plannedIncomeService.CreatePlannedIncomeAsync(cmd);
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

            var list = await _plannedIncomeService.GetPlannedIncomesByBudgetAsync(budgetId);
            foreach (var pi in list)
            {
                _console.WriteLine($"ID: {pi.Id} |Source: {pi.Source} | Date: {pi.PeriodStart:d} | Amount: {await pi.Amount.ToDisplayAsync(_currencyService)}");
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
                    var list = await _plannedIncomeService.GetPlannedIncomesByBudgetAsync(budgetId);
                    if (!list.Any())
                    {
                        _console.WriteLine("No planned incomes available to update.");
                        _console.ReadKey();
                        return;
                    }

                    foreach (var pi in list)
                    {
                        _console.WriteLine($"ID: {pi.Id} | Source: {pi.Source} | Date: {pi.PeriodStart:d} | Amount: {await pi.Amount.ToDisplayAsync(_currencyService)}");
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

                    var budget = await _budgetService.GetBudgetByIdAsync(budgetId);
                    string nativeCurrency = budget.Currency;

                    string source = _input.GetTitleInput($"Source ({existing.Source}): ");
                    DateTime period = _input.GetValidDate($"Date ({existing.PeriodStart:yyyy-MM-dd}): ", allowFuture: true);
                    decimal amount = _input.GetValidDecimal($"Amount ({await existing.Amount.ToDisplayAsync(_currencyService)}): ");

                    // 0 - Amount Rule
                    if (amount == 0)
                    {
                        bool deleted = await _plannedIncomeService.DeletePlannedIncomeAsync(id);
                        _console.WriteLine(deleted
                            ? "Planned Income deleted successfully (amount set to zero)."
                            : "Planned Income deletion failed.");
                        _console.ReadKey();
                        return;
                    }

                    var cmd = new UpdatePlannedIncomeCommand
                    {
                        Id = id,
                        Source = source,
                        PeriodStart = period,
                        Amount = new Money(amount, nativeCurrency)
                    };

                    bool ok = await _plannedIncomeService.UpdatePlannedIncomeAsync(cmd);
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

                    var list = await _plannedIncomeService.GetPlannedIncomesByBudgetAsync(budgetId);
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

                    bool ok = await _plannedIncomeService.DeletePlannedIncomeAsync(id);
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
