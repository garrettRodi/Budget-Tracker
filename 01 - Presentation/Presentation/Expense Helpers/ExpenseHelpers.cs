using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ExpenseHelpers
{
    public class ExpenseHelpers
    {
        public static async Task CreateExpense(IExpenseService expenseService, InputProcessor inputProcessor, IBudgetService budgetService)
        {
            Console.Clear();
            Console.WriteLine("=== Create Expense ===");

            var selector = new BudgetSelector(budgetService);
            Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();

            if (activeBudgetId == Guid.Empty)
            {
                Console.WriteLine("No active budget found. Please create a budget first.");
                return;
            }

            string name = inputProcessor.GetInput("Enter expense name: ");
            decimal amount = inputProcessor.GetValidDecimal("Enter amount: ");
            DateTime date = inputProcessor.GetValidDate("Enter expense date (yyyy-mm-dd): ");
            string category = inputProcessor.GetInput("Enter category (if 'Savings', this expense will update your saving goal progress): ");

            var command = new CreateExpenseCommand
            {
                BudgetContainerId = activeBudgetId,
                Name = name,
                Amount = amount,
                Date = date,
                Category = category
            };

            var expenseDto = await expenseService.CreateExpenseAsync(command);
            Console.WriteLine($"Expense '{expenseDto.Name}' created successfully with ID: {expenseDto.Id}");
        }

        public static async Task ViewExpenses(IExpenseService expenseService, IBudgetService budgetService)
        {
            Console.Clear();
            Console.WriteLine("=== View Expenses ===");

            var selector = new BudgetSelector(budgetService);
            Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();

            if (activeBudgetId == Guid.Empty)
            {
                Console.WriteLine("No active budget found. Please create a budget first.");
                return;
            }

            var expenses = await expenseService.GetExpensesByBudgetContainerIdAsync(activeBudgetId);
            foreach (var expense in expenses)
            {
                Console.WriteLine($"ID: {expense.Id} | Name: {expense.Name} | Amount: {expense.Amount} | Date: {expense.Date:d} | Category: {expense.Category}");
            }
            if (!expenses.Any())
                Console.WriteLine("No expenses found.");
        }

        public static async Task UpdateExpense(IExpenseService expenseService, InputProcessor inputProcessor, IBudgetService budgetService)
        {
            Console.Clear();
            Console.WriteLine("=== Update Expense ===");

            var selector = new BudgetSelector(budgetService);
            Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();

            if (activeBudgetId == Guid.Empty)
            {
                Console.WriteLine("No active budget found. Please create a budget first.");
                return;
            }

            Console.Write("Enter expense ID to update: ");
            if (Guid.TryParse(Console.ReadLine(), out var id))
            {
                string name = inputProcessor.GetInput("Enter updated expense name: ");
                decimal amount = inputProcessor.GetValidDecimal("Enter updated expense amount: ");
                DateTime date = inputProcessor.GetValidDate("Enter updated expense date (yyyy-mm-dd): ");
                string category = inputProcessor.GetInput("Enter updated expense category: ");

                var updateCommand = new BudgetTracker.Application.DTOs.Commands.UpdateExpenseCommand
                {
                    BudgetContainerId = activeBudgetId,
                    Id = id,
                    Name = name,
                    Amount = amount,
                    Date = date,
                    Category = category
                };

                bool result = await expenseService.UpdateExpenseAsync(updateCommand);
                Console.WriteLine(result ? "Expense updated successfully." : "Expense update failed.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        public static async Task DeleteExpense(IExpenseService expenseService, IBudgetService budgetService)
        {
            Console.Clear();
            Console.WriteLine("=== Delete Expense ===");

            var selector = new BudgetSelector(budgetService);
            Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();

            if (activeBudgetId == Guid.Empty)
            {
                Console.WriteLine("No active budget found. Please create a budget first.");
                return;
            }

            Console.Write("Enter expense ID to delete: ");
            if (Guid.TryParse(Console.ReadLine(), out var id))
            {
                bool result = await expenseService.DeleteExpenseAsync(id);
                Console.WriteLine(result ? "Expense deleted successfully." : "Expense deletion failed.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }
    }
}
