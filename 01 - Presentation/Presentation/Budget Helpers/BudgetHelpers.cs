using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.BudgetHelpers
{
    public class BudgetHelpers
    {
        public static async Task CreateBudget(IBudgetService budgetService, InputProcessor inputProcessor)
        {
            Console.Clear();
            Console.WriteLine("=== Create Budget ===");

            string name = inputProcessor.GetInput("Enter budget name: ");
            var frequency = inputProcessor.GetEnum("Enter frequency (Weekly/Monthly/Yearly): ", BudgetTracker.Domain.Entities.BudgetFrequency.Monthly);
            DateTime startDate = inputProcessor.GetValidDate("Enter start date (yyyy-mm-dd): ");
            DateTime endDate = inputProcessor.GetValidDate("Enter end date (yyyy-mm-dd): ");
            bool autoRenew = inputProcessor.GetBool("Auto renew? (y/n): ");

            // Collect budget items
            int itemCount = inputProcessor.GetValidInt("Enter number of budget items: ");
            var budgetItems = new List<CreateBudgetItemCommand>();

            for (int i = 0; i < itemCount; i++)
            {
                Console.WriteLine($"--- Budget Item {i + 1} ---");
                string category = inputProcessor.GetInput("Enter budgetItem category: ");
                decimal plannedAmount = inputProcessor.GetValidDecimal("Enter planned/budgeted amount: ");

                var itemCommand = new CreateBudgetItemCommand
                {
                    Category = category,
                    PlannedAmount = plannedAmount
                };
                budgetItems.Add(itemCommand);
            }

            var createCommand = new CreateBudgetCommand
            {
                Name = name,
                Frequency = frequency,
                StartDate = startDate,
                EndDate = endDate,
                AutoRenew = autoRenew,
                Items = budgetItems
            };

            var result = await budgetService.CreateBudgetAsync(createCommand);
            Console.WriteLine($"Budget '{result.Name}' created successfully with ID: {result.Id}");
        }

        public static async Task ViewBudgets(IBudgetService budgetService)
        {
            Console.Clear();
            Console.WriteLine("=== View Budgets ===");
            var budgets = await budgetService.GetAllBudgetsAsync();
            foreach (var budget in budgets)
            {
                Console.WriteLine($"ID: {budget.Id} | Name: {budget.Name} | Frequency: {budget.Frequency} | " +
                    $"Start: {budget.StartDate:d} | End: {budget.EndDate:d} | AutoRenew: {budget.AutoRenew}");
            }
            if (!budgets.Any())
                Console.WriteLine("No budgets found.");
        }

        public static async Task UpdateBudget(IBudgetService budgetService, InputProcessor inputProcessor)
        {
            Console.Clear();
            Console.WriteLine("=== Update Budget ===");
            Console.Write("Enter budget ID to update: ");
            if (Guid.TryParse(Console.ReadLine(), out var id))
            {
                string name = inputProcessor.GetInput("Enter updated budget name: ");
                var frequency = inputProcessor.GetEnum("Enter updated frequency (Weekly/Monthly/Yearly): ", BudgetTracker.Domain.Entities.BudgetFrequency.Monthly);
                DateTime startDate = inputProcessor.GetValidDate("Enter updated start date (yyyy-mm-dd): ");
                DateTime endDate = inputProcessor.GetValidDate("Enter updated end date (yyyy-mm-dd): ");
                bool autoRenew = inputProcessor.GetBool("Auto renew? (y/n): ");

                var updateCommand = new UpdateBudgetCommand
                {
                    Id = id,
                    Name = name,
                    Frequency = frequency,
                    StartDate = startDate,
                    EndDate = endDate,
                    AutoRenew = autoRenew
                };

                bool result = await budgetService.UpdateBudgetAsync(updateCommand);
                Console.WriteLine(result ? "Budget updated successfully." : "Budget update failed.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        public static async Task DeleteBudget(IBudgetService budgetService, InputProcessor inputProcessor)
        {
            Console.Clear();
            Console.WriteLine("=== Delete Budget ===");
            Console.Write("Enter budget ID to delete: ");
            if (Guid.TryParse(Console.ReadLine(), out var id))
            {
                bool result = await budgetService.DeleteBudgetAsync(id);
                Console.WriteLine(result ? "Budget deleted successfully." : "Budget deletion failed.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }
    }
}
