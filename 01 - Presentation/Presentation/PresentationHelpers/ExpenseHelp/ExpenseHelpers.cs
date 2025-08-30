// File: Presentation/ExpenseHelpers/ExpenseHelpers.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Helpers;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Services;
using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Presentation.UIHelpers;

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
            bool create;
            do
            {
                try
                {

                    _console.Clear();
                    _console.WriteLine("=== Create Expense ===");

                    var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
                    if (budgetId == Guid.Empty) return;

                    string name = _input.GetInput("Enter expense name: ");
                    decimal amount = _input.GetValidDecimal("Enter amount: ");
                    DateTime date = _input.GetValidDate("Enter expense date (yyyy-MM-dd): ");
                    string category = _input.GetTitleInput("Enter category: ");

                    var budget = await _budgetService.GetBudgetByIdAsync(budgetId);
                    string nativeCurrency = budget.Currency;

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
                    }
                    var cmd = new CreateExpenseCommand
                    {
                        BudgetContainerId = budgetId,
                        Name = name,
                        Amount = new Money(amount, nativeCurrency),
                        Date = date,
                        Category = category,
                        SavingGoalId = savingGoalId
                    };
                    _console.WriteLine($"[DEBUG] About to call ExpenseService.CreateExpenseAsync with SavingGoalId: {cmd.SavingGoalId}");

                    var dto = await _expenseService.CreateExpenseAsync(cmd);
                    _console.WriteLine($"Expense '{dto.Name}' created with ID: {dto.Id}");
                    _console.ReadKey();
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"Error creating expense: {ex.Message}");
                    _console.ReadKey();
                }
                create = _input.GetBool("Do you want to create another expense? (y/n): ");
            } while (create);
        }

        public async Task ViewExpensesAsync()
        {
            _console.Clear();
            _console.WriteLine("=== View Expenses ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty)
            {
                _console.WriteLine("No active budget found.");
                _console.ReadKey();
                return;
            }

            var list = await _expenseService.GetExpensesByBudgetContainerIdAsync(budgetId);

            if (!list.Any())
            {
                _console.WriteLine("No expenses found for the active budget.");
                _console.ReadKey();
                return;
            }
            else
            {
                foreach (var exp in list)
                {
                    _console.WriteLine(
                        $"ID: {exp.Id} | Name: {exp.Name} | Amount: {await exp.Amount.ToDisplayAsync(_currencyService)} | Date: {exp.Date:yyyy-MM-dd} | " +
                        $"Category: {exp.Category}" +
                        (exp.SavingGoalId.HasValue ? $" | Saving Goal ID: {exp.SavingGoalId.Value}" : ""));
                }
            }
            _console.WriteLine("\nPress any key to return...");
            _console.ReadKey();
        }

        public async Task UpdateExpenseAsync()
        {
            bool update;
            do
            {
                try
                {

                    _console.Clear();
                    _console.WriteLine("=== Update Expense ===");

                    var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
                    if (budgetId == Guid.Empty)
                    {
                        _console.WriteLine("No active budget found.");
                        _console.ReadKey();
                        return;
                    }

                    var all = (await _expenseService.GetExpensesByBudgetContainerIdAsync(budgetId)).ToList();
                    if (!all.Any())
                    {
                        _console.WriteLine("No expenses found for the active budget.");
                        _console.ReadKey();
                        return;
                    }
                    foreach (var exp in all)
                    {
                        _console.WriteLine($"ID: {exp.Id} | Name: {exp.Name} | Amount: {await exp.Amount.ToDisplayAsync(_currencyService)} | Date: {exp.Date:yyyy-MM-dd} | Category: {exp.Category}");
                    }

                    Guid id;
                    while (true)
                    {
                        id = _input.GetValidGuid("Enter expense ID to update: ");
                        if (all.Any(e => e.Id == id)) break;
                        _console.WriteLine("Invalid ID. Please choose from the list above.");
                    }

                    var existing = all.First(e => e.Id == id);

                    // 1. Prompt for new values.
                    string name = _input.GetTitleInput($"Name ({existing.Name}): ");
                    string category = _input.GetTitleInput($"Category ({existing.Category}): ");
                    decimal amount = _input.GetValidDecimal($"Amount ({await existing.Amount.ToDisplayAsync(_currencyService)}): ");
                    DateTime date = _input.GetValidDate($"Date ({existing.Date:yyyy-MM-dd}): ");

                    var budget = await _budgetService.GetBudgetByIdAsync(budgetId);
                    string nativeCurrency = budget.Currency;

                    // 0-Amount Rule
                    if (amount == 0)
                    {
                        bool deleted = await _expenseService.DeleteExpenseAsync(id);
                        _console.WriteLine(deleted
                            ? "Expense deleted successfully (amount set to zero)."
                            : "Expense deletion failed.");
                        _console.ReadKey();
                        return;
                    }

                    // Handle Savings Category 
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
                        Name = string.IsNullOrWhiteSpace(name) ? existing.Name : name,
                        Category = string.IsNullOrWhiteSpace(category) ? existing.Category : category,
                        Amount = new Money(amount, nativeCurrency),
                        Date = date,
                        SavingGoalId = savingGoalId
                    };

                    bool ok = await _expenseService.UpdateExpenseAsync(cmd);
                    _console.WriteLine(ok ? "Expense updated successfully." : "Expense update failed.");
                    _console.ReadKey();
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"Error updating expense: {ex.Message}");
                    _console.ReadKey();
                }
                update = _input.GetBool("Do you want to update another expense? (y/n): ");
            } while (update);
        }
        public async Task DeleteExpenseAsync()
        {
            bool delete;
            do
            {
                try
                {

                    _console.Clear();
                    _console.WriteLine("=== Delete Expense ===");

                    var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
                    if (budgetId == Guid.Empty)
                    {
                        _console.WriteLine("No active budget found.");
                        _console.ReadKey();
                        return;
                    }

                    var all = (await _expenseService.GetExpensesByBudgetContainerIdAsync(budgetId)).ToList();
                    if (!all.Any())
                    {
                        _console.WriteLine("No expenses found for the active budget.");
                        _console.ReadKey();
                        return;
                    }

                    foreach (var exp in all)
                    {
                        _console.WriteLine($"ID: {exp.Id} | Name: {exp.Name} | Amount: {exp.Amount:C} | Date: {exp.Date:yyyy-MM-dd} | Category: {exp.Category}");
                    }

                    Guid id;
                    while (true)
                    {
                        id = _input.GetValidGuid("Enter expense ID to delete: ");
                        if (all.Any(e => e.Id == id)) break;
                        _console.WriteLine("Invalid ID. Please choose from the list above.");
                    }

                    var expense = all.First(e => e.Id == id);

                    bool success = await _expenseService.DeleteExpenseAsync(id);
                    // Recalculate saving goal if applicable
                    if (success && expense.Category.Equals("Savings", StringComparison.OrdinalIgnoreCase))
                    {
                        var savingGoal = await _savingGoalsService.GetSavingGoalByIdAsync(expense.SavingGoalId.Value);
                        if (expense.SavingGoalId.HasValue)
                        {
                            await _savingGoalsService.RecalculateCurrentAmountAsync(savingGoal.Id);
                        }
                    }

                    _console.WriteLine(success
                        ? "Expense deleted successfully."
                        : "Failed to delete expense.");
                    _console.ReadKey();
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"Error deleting expense: {ex.Message}");
                    _console.ReadKey();
                }
                delete = _input.GetBool("Do you want to delete another expense? (y/n): ");
            } while (delete);
        }
    }
}
