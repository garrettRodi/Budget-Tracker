// File: Presentation/PresentationHelpers/PlannedExpenseHelpers.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Services;
using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.PresentationHelpers
{
    public class PlannedExpenseHelpers
    {
        private readonly IPlannedExpenseService _plannedExpenseService;
        private readonly SelectBudgetContainer _selector;
        private readonly InputProcessor _input;
        private readonly IConsole _console;
        private readonly ICurrencyService _currencyService;

        public PlannedExpenseHelpers(
            IPlannedExpenseService plannedExpenseService,
            SelectBudgetContainer selector,
            InputProcessor input,
            IConsole console,
            ICurrencyService currencyService)
        {
            _plannedExpenseService = plannedExpenseService
                ?? throw new ArgumentNullException(nameof(plannedExpenseService));
            _selector = selector
                ?? throw new ArgumentNullException(nameof(selector));
            _input = input
                ?? throw new ArgumentNullException(nameof(input));
            _console = console
                ?? throw new ArgumentNullException(nameof(console));
            _currencyService = currencyService;
        }

        public async Task CreatePlannedExpenseAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Create Planned Expense ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty)
            {
                _console.WriteLine("No active budget found. Please create a budget first.");
                return;
            }

            string name = _input.GetTitleInput("Enter name for planned expense: ");
            string category = _input.GetTitleInput("Enter category: ");
            decimal amount = _input.GetValidDecimal("Enter planned amount: ");
            DateTime period = _input.GetValidDate("Enter period (yyyy-MM-dd): ", allowFuture: true);

            var cmd = new CreatePlannedExpenseCommand
            {
                BudgetContainerId = budgetId,
                Name = name,
                Category = category,
                Amount = new Money(amount, _currencyService.CurrentCurrency),
                Period = period
            };

            try
            {
                var dto = await _plannedExpenseService.CreatePlannedExpenseAsync(cmd);
                _console.WriteLine($"Planned expense '{dto.Category}' on {dto.Period:yyyy-MM-dd} for {dto.Amount:C} created with ID: {dto.Id}");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error creating planned expense: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }

            _console.WriteLine("Press any key to return...");
            _console.ReadKey();
        }

        public async Task ViewPlannedExpensesAsync()
        {
            _console.Clear();
            _console.WriteLine("=== View Planned Expenses ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty)
            {
                _console.WriteLine("No active budget found. Please create a budget first.");
                _console.ReadKey();
                return;
            }
            try
            {
                var list = await _plannedExpenseService.ViewPlannedExpensesAsync(budgetId);
                if (!list.Any())
                {
                    _console.WriteLine("No expenses found.");
                }
                else
                {

                    foreach (var exp in list)
                    {
                        _console.WriteLine(
                            $"ID: {exp.Id} | Name: {exp.Name} | Amount: {exp.Amount:C} | Date: {exp.Period:yyyy-MM-dd} | Category: {exp.Category}");
                    }
                }
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error retrieving planned expenses: {ex.Message}");
            }
            _console.WriteLine("\nPress any key to return...");
            _console.ReadKey();
        }
        public async Task UpdatePlannedExpenseAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Update Planned Expense ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty)
            {
                _console.WriteLine("No active budget found. Please create a budget first.");
                return;
            }

            var all = await _plannedExpenseService.ViewPlannedExpensesAsync(budgetId);
            if (!all.Any())
            {
                _console.WriteLine("No planned expenses to update.");
                return;
            }

            // Display all planned expenses for user to choose from
            foreach (var pe in all)
                _console.WriteLine($"{pe.Id}: {pe.Name} | {pe.Period:yyyy-MM-dd} | {pe.Category} | {pe.Amount:C}");

            Guid id;
            while (true)
            {
                id = _input.GetValidGuid("Enter the ID of the planned expense to update: ");
                if (all.Any(pe => pe.Id == id)) break;
                _console.WriteLine("Invalid ID. Please choose one from the list above.");
            }

            var existing = all.First(pe => pe.Id == id);

            string name = _input.GetTitleInput($"Name ({existing.Name}): ");
            string category = _input.GetTitleInput($"Category ({existing.Category}): ");
            decimal amount = _input.GetValidDecimal($"Amount ({existing.Amount:C}): ");
            DateTime period = _input.GetValidDate($"Period ({existing.Period:yyyy-MM-dd}): ", allowFuture: true);

            var cmd = new UpdatePlannedExpenseCommand
            {
                Id = id,
                BudgetContainerId = budgetId,
                Name = string.IsNullOrWhiteSpace(name) ? existing.Name : name,
                Category = string.IsNullOrWhiteSpace(category) ? existing.Category : category,
                Amount = new Money(amount, _currencyService.CurrentCurrency),
                Period = period
            };

            bool ok = await _plannedExpenseService.UpdatePlannedExpenseAsync(cmd);
            _console.WriteLine(ok ? "Planned expense updated successfully." : "Failed to update planned expense.");
            _console.ReadKey();
        }

        public async Task DeletePlannedExpenseAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Delete Planned Expense ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty)
            {
                _console.WriteLine("No active budget found. Please create a budget first.");
                return;
            }

            var all = await _plannedExpenseService.ViewPlannedExpensesAsync(budgetId);
            if (!all.Any())
            {
                _console.WriteLine("No planned expenses to delete.");
                return;
            }

            foreach (var pe in all)
                _console.WriteLine($"{pe.Id}: {pe.Period:yyyy-MM-dd} | {pe.Category} | {pe.Amount:C}");

            Guid id;
            while (true)
            {
                id = _input.GetValidGuid("Enter the ID of the planned expense to delete: ");
                if (all.Any(pe => pe.Id == id)) break;
                _console.WriteLine("Invalid ID. Please choose one from the list above.");
            }

            bool ok = await _plannedExpenseService.DeletePlannedExpenseAsync(id);
            _console.WriteLine(ok ? "Planned expense deleted." : "Failed to delete planned expense.");
            _console.ReadKey();
        }
    }
}
