using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.IncomeHelpers
{
    public class IncomeHelpers
    {
        public static async Task CreateIncome(IIncomeService incomeService, InputProcessor inputProcessor, IBudgetService budgetService)
        {
            Console.Clear();
            Console.WriteLine("=== Create Income ===");

            var selector = new BudgetSelector(budgetService);
            Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();

            if (activeBudgetId == Guid.Empty)
            {
                Console.WriteLine("No active budget found. Please create a budget first.");
                return;
            }

            string source = inputProcessor.GetInput("Enter income source (i.e., Salary, Bonus): ");

            decimal amount = inputProcessor.GetValidDecimal("Enter the actual amount recieved: ");

            DateTime date = inputProcessor.GetValidDate("Enter recieved date (yyyy-mm-dd): ");

            var command = new CreateIncomeCommand
            {
                BudgetContainerId = activeBudgetId,
                Source = source,
                ActualAmount = amount,
                ReceivedDate = date
            };
            var incomeDto = await incomeService.CreateIncomeAsync(command);
            Console.WriteLine($"Income from '{incomeDto.Source}' created successfully with ID: {incomeDto.Id}");
        }

        public static async Task ViewIncomes(IIncomeService incomeService, IBudgetService budgetService)
        {
            Console.Clear();
            Console.WriteLine("=== View Incomes ===");

            var selector = new BudgetSelector(budgetService);
            Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();

            if (activeBudgetId == Guid.Empty)
            {
                Console.WriteLine("No active budget found. Please create a budget first.");
                return;
            }

            var incomes = await incomeService.GetIncomesByBudgetContainerIdAsync(activeBudgetId);
            foreach (var income in incomes)
            {
                Console.WriteLine($"ID: {income.Id} | Source: {income.Source} | Amount: {income.ActualAmount} | Date: {income.ReceivedDate:d}");
            }
            if (!incomes.Any())
                Console.WriteLine("No incomes found for the active budget.");
        }

        public static async Task UpdateIncome(IIncomeService incomeService, InputProcessor inputProcessor, IBudgetService budgetService)
        {
            Console.Clear();
            Console.WriteLine("=== Update Income ===");

            var selector = new BudgetSelector(budgetService);
            Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();

            if (activeBudgetId == Guid.Empty)
            {
                Console.WriteLine("No active budget found. Please create a budget first.");
                return;
            }

            Console.Write("Enter income ID to update: ");
            if (Guid.TryParse(Console.ReadLine(), out var id))
            {
                string source = inputProcessor.GetInput("Enter updated income source: ");
                decimal amount = inputProcessor.GetValidDecimal("Enter updated actual amount recieved: ");
                DateTime date = inputProcessor.GetValidDate("Enter updated recieved date (yyyy-mm-dd): ");
                var updateCommand = new BudgetTracker.Application.DTOs.Commands.UpdateIncomeCommand
                {
                    Id = id,
                    Source = source,
                    ActualAmount = amount,
                    ReceivedDate = date
                };
                bool result = await incomeService.UpdateIncomeAsync(updateCommand);
                Console.WriteLine(result ? "Income updated successfully." : "Income update failed.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        public static async Task DeleteIncome(IIncomeService incomeService, InputProcessor inputProcessor, IBudgetService budgetService)
        {
            Console.Clear();
            Console.WriteLine("=== Delete Income ===");

            var selector = new BudgetSelector(budgetService);
            Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();

            if (activeBudgetId == Guid.Empty)
            {
                Console.WriteLine("No active budget found. Please create a budget first.");
                return;
            }

            Console.Write("Enter income ID to delete: ");
            if (Guid.TryParse(Console.ReadLine(), out var id))
            {
                bool result = await incomeService.DeleteIncomeAsync(id);
                Console.WriteLine(result ? "Income deleted successfully." : "Income deletion failed.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }
    }
}
