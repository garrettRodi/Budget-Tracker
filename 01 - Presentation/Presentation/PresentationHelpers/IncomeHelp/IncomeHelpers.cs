// File: Presentation/IncomeHelpers/IncomeHelpers.cs
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
    public class IncomeHelpers
    {
        private readonly IIncomeService _incomeService;
        private readonly SelectBudgetContainer _selector;
        private readonly InputProcessor _input;
        private readonly IConsole _console;
        private readonly ICurrencyService _currencyService;

        public IncomeHelpers(
            IIncomeService incomeService,
            SelectBudgetContainer selector,
            InputProcessor input,
            IConsole console,
            ICurrencyService currencyService)
        {
            _incomeService = incomeService
                ?? throw new ArgumentNullException(nameof(incomeService));
            _selector = selector
                ?? throw new ArgumentNullException(nameof(selector));
            _input = input
                ?? throw new ArgumentNullException(nameof(input));
            _console = console
                ?? throw new ArgumentNullException(nameof(console));
            _currencyService = currencyService;
        }

        public async Task CreateIncomeAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Create Income ===");
            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            string source = _input.GetTitleInput("Enter income source (e.g., Salary, Bonus): ");
            decimal amount = _input.GetValidDecimal("Enter the actual amount received: ");
            DateTime date = _input.GetValidDate("Enter received date (yyyy-MM-dd): ");

            var cmd = new CreateIncomeCommand
            {
                BudgetContainerId = budgetId,
                Source = source,
                ActualAmount = new Money(amount, _currencyService.CurrentCurrency),
                ReceivedDate = date
            };

            var dto = await _incomeService.CreateIncomeAsync(cmd);
            _console.WriteLine($"Income from '{dto.Source}' created successfully with ID: {dto.Id}");
            _console.ReadKey();
        }

        public async Task ViewIncomesAsync()
        {
            _console.Clear();
            _console.WriteLine("=== View Incomes ===");
            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var list = await _incomeService.GetIncomesByBudgetContainerIdAsync(budgetId);
            foreach (var inc in list)
            {
                _console.WriteLine(
                $"ID: {inc.Id} | Source: {inc.Source} | Amount: {inc.ActualAmount} | Date: {inc.ReceivedDate:yyyy-MM-dd}");
            }
            if (!list.Any())
                _console.WriteLine("No incomes found for the active budget.");
            _console.ReadKey();
        }

        public async Task UpdateIncomeAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Update Income ===");
            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty)
            {
                _console.WriteLine("No active budget found. Please create or select a budget first.");
                _console.ReadKey();
                return;
            }

            var list = (await _incomeService.GetIncomesByBudgetContainerIdAsync(budgetId)).ToList();
            if (!list.Any())
            {
                _console.WriteLine("No incomes found for the active budget.");
                _console.ReadKey();
                return;
            }

            foreach (var inc in list)
            {
                _console.WriteLine($"ID: {inc.Id} | Source: {inc.Source} | Amount: {inc.ActualAmount:C} | Date: {inc.ReceivedDate:yyyy-MM-dd}");
            }

            Guid id;
            while (true)
            {
                id = _input.GetValidGuid("Enter income ID to update: ");
                if (list.Any(i => i.Id == id)) break;
                _console.WriteLine("Invalid ID. Please choose from the list above.");
            }

            var existing = list.First(i => i.Id == id);

            string source = _input.GetTitleInput($"Source ({existing.Source}): ");
            decimal amount = _input.GetValidDecimal($"Amount ({existing.ActualAmount:C}): ");
            DateTime date = _input.GetValidDate($"Date ({existing.ReceivedDate:yyyy-MM-dd}): ");

            var cmd = new UpdateIncomeCommand
            {
                BudgetContainerId = budgetId,
                Id = id,
                Source = string.IsNullOrWhiteSpace(source) ? existing.Source : source,
                ActualAmount = new Money(amount, _currencyService.CurrentCurrency),
                ReceivedDate = date
            };

            bool success = await _incomeService.UpdateIncomeAsync(cmd);
            _console.WriteLine(success
                ? "Income updated successfully."
                : "Income update failed.");
            _console.ReadKey();
        }

        public async Task DeleteIncomeAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Delete Income ===");
            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty)
            {
                _console.WriteLine("No active budget found. Please create or select a budget first.");
                _console.ReadKey();
                return;
            }

            var list = (await _incomeService.GetIncomesByBudgetContainerIdAsync(budgetId)).ToList();
            if (!list.Any())
            {
                _console.WriteLine("No incomes found for the active budget.");
                _console.ReadKey();
                return;
            }

            foreach (var inc in list)
            {
                _console.WriteLine($"ID: {inc.Id} | Source: {inc.Source} | Amount: {inc.ActualAmount:C} | Date: {inc.ReceivedDate:yyyy-MM-dd}");
            }

            Guid id;
            while (true)
            {
                id = _input.GetValidGuid("Enter income ID to delete: ");
                if (list.Any(i => i.Id == id)) break;
                _console.WriteLine("Invalid ID. Please choose from the list above.");
            }

            bool success = await _incomeService.DeleteIncomeAsync(id);
            _console.WriteLine(success
                ? "Income deleted successfully."
                : "Income deletion failed.");
            _console.ReadKey();
        }
    }
}
