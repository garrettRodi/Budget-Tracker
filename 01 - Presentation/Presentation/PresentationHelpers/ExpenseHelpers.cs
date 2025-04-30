// File: Presentation/ExpenseHelpers/ExpenseHelpers.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.PresentationHelpers
{
    public class ExpenseHelpers
    {
        private readonly IExpenseService _expenseService;
        private readonly IBudgetService _budgetService;
        private readonly SelectBudgetContainer _selector;
        private readonly InputProcessor _input;
        private readonly IConsole _console;

        public ExpenseHelpers(
            IExpenseService expenseService,
            IBudgetService budgetService,
            SelectBudgetContainer selector,
            InputProcessor input,
            IConsole console)
        {
            _expenseService = expenseService
                ?? throw new ArgumentNullException(nameof(expenseService));
            _budgetService = budgetService
                ?? throw new ArgumentNullException(nameof(budgetService));
            _selector = selector
                ?? throw new ArgumentNullException(nameof(selector));
            _input = input
                ?? throw new ArgumentNullException(nameof(input));
            _console = console
                ?? throw new ArgumentNullException(nameof(console));
        }

        public async Task CreateExpenseAsync()
        {
            _console.WriteLine("=== Create Expense ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            string name = _input.GetInput("Enter expense name: ");
            decimal amount = _input.GetValidDecimal("Enter amount: ");
            DateTime date = _input.GetValidDate("Enter expense date (yyyy-MM-dd): ");
            string category = _input.GetInput("Enter category: ");

            var cmd = new CreateExpenseCommand
            {
                BudgetContainerId = budgetId,
                Name = name,
                Amount = amount,
                Date = date,
                Category = category
            };

            var dto = await _expenseService.CreateExpenseAsync(cmd);
            _console.WriteLine($"Expense '{dto.Name}' created with ID: {dto.Id}");
        }

        public async Task ViewExpensesAsync()
        {
            _console.WriteLine("=== View Expenses ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var list = await _expenseService.GetExpensesByBudgetContainerIdAsync(budgetId);
            foreach (var exp in list)
            {
                _console.WriteLine(
                    $"ID: {exp.Id} | Name: {exp.Name} | Amount: {exp.Amount:C} | Date: {exp.Date:yyyy-MM-dd} | Category: {exp.Category}");
            }
            if (!list.Any())
                _console.WriteLine("No expenses found.");
        }

        public async Task UpdateExpenseAsync()
        {
            _console.WriteLine("=== Update Expense ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var idInput = _input.GetInput("Enter expense ID to update: ");
            if (!Guid.TryParse(idInput, out var id))
            {
                _console.WriteLine("Invalid ID format.");
                return;
            }

            string name = _input.GetInput("Enter updated expense name: ");
            decimal amount = _input.GetValidDecimal("Enter updated amount: ");
            DateTime date = _input.GetValidDate("Enter updated date (yyyy-MM-dd): ");
            string category = _input.GetInput("Enter updated category: ");

            var cmd = new UpdateExpenseCommand
            {
                BudgetContainerId = budgetId,
                Id = id,
                Name = name,
                Amount = amount,
                Date = date,
                Category = category
            };

            bool success = await _expenseService.UpdateExpenseAsync(cmd);
            _console.WriteLine(success
                ? "Expense updated successfully."
                : "Expense update failed.");
        }

        public async Task DeleteExpenseAsync()
        {
            _console.WriteLine("=== Delete Expense ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var idInput = _input.GetInput("Enter expense ID to delete: ");
            if (!Guid.TryParse(idInput, out var id))
            {
                _console.WriteLine("Invalid ID format.");
                return;
            }

            bool success = await _expenseService.DeleteExpenseAsync(id);
            _console.WriteLine(success
                ? "Expense deleted successfully."
                : "Expense deletion failed.");
        }
    }
}
