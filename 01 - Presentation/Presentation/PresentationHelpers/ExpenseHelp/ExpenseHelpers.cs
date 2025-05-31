// File: Presentation/ExpenseHelpers/ExpenseHelpers.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Helpers;
using BudgetTracker.Presentation.UIHelpers;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Presentation.PresentationHelpers
{
    public class ExpenseHelpers
    {
        private readonly IExpenseService _expenseService;
        private readonly IBudgetService _budgetService;
        private readonly ISavingGoalsService _savingGoalsService;
        private readonly SelectBudgetContainer _selector;
        private readonly InputProcessor _input;
        private readonly IConsole _console;
        private readonly ICurrencyService _currencyService;

        public ExpenseHelpers(
            IExpenseService expenseService,
            IBudgetService budgetService,
            ISavingGoalsService savingGoalsService,
            SelectBudgetContainer selector,
            InputProcessor input,
            IConsole console,
            ICurrencyService currencyService)
        {
            _expenseService = expenseService
                ?? throw new ArgumentNullException(nameof(expenseService));
            _budgetService = budgetService
                ?? throw new ArgumentNullException(nameof(budgetService));
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

        public async Task CreateExpenseAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Create Expense ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            string name = _input.GetInput("Enter expense name: ");
            decimal amount = _input.GetValidDecimal("Enter amount: ");
            DateTime date = _input.GetValidDate("Enter expense date (yyyy-MM-dd): ");
            string category = _input.GetTitleInput("Enter category: ");

            Guid? savingGoalId = null;
            if (category.Equals("Savings", StringComparison.OrdinalIgnoreCase))
            {
                // Fetch all saving goals for this budget
                var savingGoals = (await _savingGoalsService.GetSavingGoalsByBudgetContainerIdAsync(budgetId)).ToList();

                if (savingGoals.Any())
                {
                    _console.WriteLine("Select the Saving Goal to allocate this savings expense to:");
                    for (int i = 0; i < savingGoals.Count; i++)
                    {
                        var g = savingGoals[i];
                        _console.WriteLine($"{i + 1}. {g.GoalName} (IDictionary: {g.Id})");
                    }
                    int selected = -1;
                    while (selected < 1 || selected > savingGoals.Count)
                    {
                        selected = _input.GetValidInt("Enter the number of the saving goal: ");
                    }
                    savingGoalId = savingGoals[selected - 1].Id;
                }
                else
                {
                    _console.WriteLine("No saving goals found. This savings expense will not be allocated to a goal (Bulk Savings).");
                }
                _console.WriteLine($"[DEBUG] SavingGoalId selected: {savingGoalId}");
            }
            var cmd = new CreateExpenseCommand
            {
                BudgetContainerId = budgetId,
                Name = name,
                Amount = new Money(amount, _currencyService.CurrentCurrency),
                Date = date,
                Category = category,
                SavingGoalId = savingGoalId
            };
            _console.WriteLine($"[DEBUG] About to call ExpenseService.CreateExpenseAsync with SavingGoalId: {cmd.SavingGoalId}");

            var dto = await _expenseService.CreateExpenseAsync(cmd);
            _console.WriteLine($"Expense '{dto.Name}' created with ID: {dto.Id}");
            _console.ReadKey();
        }

        public async Task ViewExpensesAsync()
        {
            _console.Clear();
            _console.WriteLine("=== View Expenses ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var list = await _expenseService.GetExpensesByBudgetContainerIdAsync(budgetId);

            _console.Clear();
            foreach (var exp in list)
            {
                _console.WriteLine(
                    $"ID: {exp.Id} | Name: {exp.Name} | Amount: {exp.Amount:C} | Date: {exp.Date:yyyy-MM-dd} | Category: {exp.Category}");
            }
            if (!list.Any())
                _console.WriteLine("No expenses found.");
            _console.ReadKey();
        }

        public async Task UpdateExpenseAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Update Expense ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var idInput = _input.GetInput("Enter expense ID to update: ");
            if (!Guid.TryParse(idInput, out var id))
            {
                _console.WriteLine("Invalid ID format.");
                return;
            }

            // 1. Prompt for new values.
            string name = _input.GetInput("Enter updated expense name: ");
            decimal amount = _input.GetValidDecimal("Enter updated amount: ");
            DateTime date = _input.GetValidDate("Enter updated date (yyyy-MM-dd): ");
            string category = _input.GetTitleInput("Enter updated category: ");


            // 2. If amount is zero, treat as delete (per your requirement).
            if (amount == 0)
            {
                bool deleted = await _expenseService.DeleteExpenseAsync(id);
                _console.WriteLine(deleted
                    ? "Expense deleted successfully (amount set to zero)."
                    : "Expense deletion failed.");
                _console.ReadKey();
                return;
            }

            // 3. SavingGoalId logic: only ask if category == "Savings".
            Guid? savingGoalId = null;
            if (category.Equals("Savings", StringComparison.OrdinalIgnoreCase))
            {
                // Fetch saving goals for this budget.
                var savingGoals = (await _savingGoalsService.GetSavingGoalsByBudgetContainerIdAsync(budgetId)).ToList();
                if (savingGoals.Any())
                {
                    _console.WriteLine("Select the Saving Goal for this expense (by number):");
                    for (int i = 0; i < savingGoals.Count; i++)
                    {
                        var g = savingGoals[i];
                        _console.WriteLine($"{i + 1}. {g.GoalName} (ID: {g.Id})");
                    }
                    int selected = -1;
                    while (selected < 1 || selected > savingGoals.Count)
                    {
                        selected = _input.GetValidInt("Enter the number of the saving goal: ");
                    }
                    savingGoalId = savingGoals[selected - 1].Id;
                }
                else
                {
                    _console.WriteLine("No saving goals found. This savings expense will not be allocated to a goal (Bulk Savings).");
                }
            }

            // 4. Build update command, including SavingGoalId.
            var cmd = new UpdateExpenseCommand
            {
                BudgetContainerId = budgetId,
                Id = id,
                Name = name,
                Amount = new Money(amount, _currencyService.CurrentCurrency),
                Date = date,
                Category = category,
                SavingGoalId = savingGoalId
            };

            // 5. Call update and report result.
            _console.WriteLine($"DEBUG: Update - id={id}, name={name}, amount={amount}, date={date}, category={category}, savingGoalId={savingGoalId}");
            bool success = await _expenseService.UpdateExpenseAsync(cmd);
            _console.WriteLine(success
                ? "Expense updated successfully."
                : "Expense update failed.");
            _console.ReadKey();
        }


        public async Task DeleteExpenseAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Delete Expense ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var idInput = _input.GetInput("Enter expense ID to delete: ");
            if (!Guid.TryParse(idInput, out var id))
            {
                _console.WriteLine("Invalid ID format.");
                return;
            }

            // 1. Fetch the expense so we can access its properties before deleting
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            if (expense == null)
            {
                _console.WriteLine("Expense not found.");
                return;
            }


            // 2. Delete the expense
            bool success = await _expenseService.DeleteExpenseAsync(id);

            // 3. If this was a savings expense linked to a goal, recalculate the goal
            if (success && expense.Category.Equals("Savings", StringComparison.OrdinalIgnoreCase))
            {
                if (expense.SavingGoalId.HasValue)
                {
                    await _savingGoalsService.RecalculateCurrentAmountAsync(expense.SavingGoalId.Value);
                }
                // For bulk savings: nothing needed, since report just sums all unlinked savings
            }

            // Notify user
            _console.WriteLine(success
                ? "Expense deleted successfully."
                : "Expense deletion failed.");
            _console.ReadKey();
        }
    }
}
