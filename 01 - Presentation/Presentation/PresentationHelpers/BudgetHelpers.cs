// File: Presentation/BudgetHelpers/BudgetHelpers.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.PresentationHelpers
{
    public class BudgetHelpers
    {
        private readonly IBudgetService _budgetService;
        private readonly SelectBudgetContainer _selector;
        private readonly InputProcessor _input;
        private readonly IConsole _console;
        private readonly ICategoryMappingService _categoryMappingService;

        public BudgetHelpers(
            IBudgetService budgetService,
            SelectBudgetContainer selector,
            InputProcessor input,
            IConsole console,
            ICategoryMappingService categoryMappingService)
        {
            _budgetService = budgetService
                ?? throw new ArgumentNullException(nameof(budgetService));
            _selector = selector
                ?? throw new ArgumentNullException(nameof(selector));
            _input = input
                ?? throw new ArgumentNullException(nameof(input));
            _console = console
                ?? throw new ArgumentNullException(nameof(console));
            _categoryMappingService = categoryMappingService;
        }

        public async Task CreateBudgetAsync()
        {
            try
            {
                _console.Clear();
                _console.WriteLine("=== Create Budget ===");

                string name = _input.GetInput("Enter budget name: ");
                var frequency = _input.GetEnum(
                    "Enter frequency (Weekly/Monthly/Yearly): ",
                    Domain.Entities.BudgetFrequency.Monthly);
                DateTime startDate = _input.GetValidDate(
                    "Enter start date (yyyy-MM-dd): ");
                DateTime endDate = _input.GetValidDate(
                    "Enter end date (yyyy-MM-dd): ");
                bool autoRenew = _input.GetBool("Auto renew? (y/n): ");

                // 1) Fetch all category names from the injected service
                var allCategories = (await _categoryMappingService
                                         .GetAllCategoryNamesAsync())
                                    .ToList();

                // 2) Let the user pick categories until they type 'done'
                var items = new List<CreateBudgetItemCommand>();
                while (true)
                {
                    _console.WriteLine("--- Select a category or type 'done' ---");
                    for (int idx = 0; idx < allCategories.Count; idx++)
                    {
                        _console.WriteLine($"{idx + 1}. {allCategories[idx]}");
                    }
                    _console.WriteLine($"{allCategories.Count + 1}. Other (enter custom)");

                    string input = _input.GetInput("Choice or 'done': ").Trim();
                    if (input.Equals("done", StringComparison.OrdinalIgnoreCase))
                        break;

                    string category;
                    if (int.TryParse(input, out int choice)
                        && choice >= 1
                        && choice <= allCategories.Count)
                    {
                        category = allCategories[choice - 1];
                    }
                    else if (choice == allCategories.Count + 1
                             || !int.TryParse(input, out _))
                    {
                        category = _input.GetInput("Enter custom category name: ");
                    }
                    else
                    {
                        _console.WriteLine("Invalid selection. Try again.");
                        continue;
                    }

                    decimal plannedAmount = _input.GetValidDecimal(
                        "Enter planned amount: ");
                    items.Add(new CreateBudgetItemCommand
                    {
                        Category = category,
                        PlannedAmount = plannedAmount
                    });
                }

                if (items.Count == 0)
                {
                    _console.WriteLine(
                        "You must enter at least one budget item.");
                    return;
                }

                var cmd = new CreateBudgetCommand
                {
                    Name = name,
                    Frequency = frequency,
                    StartDate = startDate,
                    EndDate = endDate,
                    AutoRenew = autoRenew,
                    Items = items
                };

                var dto = await _budgetService.CreateBudgetAsync(cmd);
                _console.WriteLine(
                    $"Budget '{dto.Name}' created with ID: {dto.Id}");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error creating budget: {ex.Message}");
                _console.WriteLine(ex.StackTrace);
            }
        }


        public async Task ViewBudgetsAsync()
        {
            _console.Clear();
            _console.WriteLine("=== View Budgets ===");
            var budgets = await _budgetService.GetAllBudgetsAsync();
            foreach (var b in budgets)
            {
                _console.WriteLine(
                    $"ID: {b.Id} | Name: {b.Name} | Frequency: {b.Frequency} | " +
                    $"Start: {b.StartDate:yyyy-MM-dd} | End: {b.EndDate:yyyy-MM-dd} | AutoRenew: {b.AutoRenew}");
            }
            if (!budgets.Any())
                _console.WriteLine("No budgets found.");
        }

        public async Task UpdateBudgetAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Update Budget ===");
            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            string name = _input.GetInput("Enter new budget name: ");
            var frequency = _input.GetEnum("Enter new frequency (Weekly/Monthly/Yearly): ",
                Domain.Entities.BudgetFrequency.Monthly);
            DateTime startDate = _input.GetValidDate("Enter new start date (yyyy-MM-dd): ");
            DateTime endDate = _input.GetValidDate("Enter new end date (yyyy-MM-dd): ");
            bool autoRenew = _input.GetBool("Auto renew? (y/n): ");

            var cmd = new UpdateBudgetCommand
            {
                Id = budgetId,
                Name = name,
                Frequency = frequency,
                StartDate = startDate,
                EndDate = endDate,
                AutoRenew = autoRenew
            };

            bool success = await _budgetService.UpdateBudgetAsync(cmd);
            _console.WriteLine(success
                ? "Budget updated successfully."
                : "Failed to update budget.");
        }

        public async Task DeleteBudgetAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Delete Budget ===");
            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            bool success = await _budgetService.DeleteBudgetAsync(budgetId);
            _console.WriteLine(success
                ? "Budget deleted successfully."
                : "Failed to delete budget.");
        }
    }
}
