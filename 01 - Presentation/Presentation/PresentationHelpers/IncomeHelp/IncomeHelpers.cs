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
            if (budgetId == Guid.Empty) return;

            string idInput = _input.GetInput("Enter income ID to update: ");
            if (!Guid.TryParse(idInput, out var id))
            {
                _console.WriteLine("Invalid ID format.");
                return;
            }

            string source = _input.GetTitleInput("Enter updated income source: ");
            decimal amount = _input.GetValidDecimal("Enter updated amount received: ");
            DateTime date = _input.GetValidDate("Enter updated received date (yyyy-MM-dd): ");

            var cmd = new UpdateIncomeCommand
            {
                BudgetContainerId = budgetId,
                Id = id,
                Source = source,
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
            if (budgetId == Guid.Empty) return;

            string idInput = _input.GetInput("Enter income ID to delete: ");
            if (!Guid.TryParse(idInput, out var id))
            {
                _console.WriteLine("Invalid ID format.");
                return;
            }

            bool success = await _incomeService.DeleteIncomeAsync(id);
            _console.WriteLine(success
                ? "Income deleted successfully."
                : "Income deletion failed.");
            _console.ReadKey();
        }
    }
}
