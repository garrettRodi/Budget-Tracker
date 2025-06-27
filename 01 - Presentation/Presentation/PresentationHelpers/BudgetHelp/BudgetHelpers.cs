// File: Presentation/BudgetHelpers/BudgetHelpers.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;
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
        private readonly ICurrencyService _currencyService;

        public BudgetHelpers(
            IBudgetService budgetService,
            SelectBudgetContainer selector,
            InputProcessor input,
            IConsole console,
            ICategoryMappingService categoryMappingService,
            ICurrencyService currencyService)
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
            _currencyService = currencyService;
        }

        public async Task CreateBudgetAsync()
        {
            try
            {
                _console.Clear();
                _console.WriteLine("=== Create Budget ===");

                string name = _input.GetTitleInput("Enter budget name: ");
                var frequency = _input.GetEnum<BudgetFrequency>("Enter frequency (Weekly, Monthly, Yearly): ");
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

                    string input = _input.GetTitleInput("Choice or 'done': ").Trim();

                    if (input.Equals("done", StringComparison.OrdinalIgnoreCase))
                    {
                        if (items.Count == 0)
                        {
                            _console.WriteLine("You must enter at least one budget item before finishing.");
                            continue; // Loop again instead of returning
                        }

                        break; // Exit loop if at least one item exists
                    }

                    string category;
                    if (int.TryParse(input, out int choice)
                        && choice >= 1
                        && choice <= allCategories.Count)
                    {
                        category = allCategories[choice - 1];
                    }
                    else if (choice == allCategories.Count + 1 || !int.TryParse(input, out _))
                    {
                        category = _input.GetTitleInput("Enter custom category name: ");
                    }
                    else
                    {
                        _console.WriteLine("Invalid selection. Try again.");
                        continue;
                    }

                    decimal plannedAmount = _input.GetValidDecimal("Enter planned amount: ");

                    items.Add(new CreateBudgetItemCommand
                    {
                        Category = category,
                        PlannedAmount = new Money(plannedAmount, _currencyService.CurrentCurrency)
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
            _console.ReadKey();
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
            _console.ReadKey();
        }

        public async Task UpdateBudgetAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Update Budget ===");

            var budgets = (await _budgetService.GetAllBudgetsAsync()).ToList(); // ✅ Correct

            if (!budgets.Any())
            {
                _console.WriteLine("No budgets available to update.");
                _console.ReadKey();
                return;
            }
            foreach (var b in budgets)
            {
                _console.WriteLine(
                    $"ID: {b.Id} | Name: {b.Name} | Frequency: {b.Frequency} | " +
                    $"Start: {b.StartDate:yyyy-MM-dd} | End: {b.EndDate:yyyy-MM-dd} | AutoRenew: {b.AutoRenew}");
            }
            Guid id;
            while (true)
            {
                id = _input.GetValidGuid("Enter the ID of the budget to update: ");
                if (budgets.Any(b => b.Id == id)) break;
                _console.WriteLine("Invalid ID. Please select one from the list above.");
            }
            var existing = budgets.First(b => b.Id == id);

            string name = _input.GetTitleInput($"Name ({existing.Name}): ");
            var frequency = _input.GetEnum<BudgetFrequency>($"Frequency ({existing.Frequency}): ");
            DateTime startDate = _input.GetValidDate($"Start Date ({existing.StartDate:yyyy-MM-dd}): ");
            DateTime endDate = _input.GetValidDate($"End Date ({existing.EndDate:yyyy-MM-dd}): ");
            bool autoRenew = _input.GetBool($"Auto Renew ({(existing.AutoRenew ? "y" : "n")}): ");

            var cmd = new UpdateBudgetCommand
            {
                Id = id,
                Name = string.IsNullOrWhiteSpace(name) ? existing.Name : name,
                Frequency = frequency,
                StartDate = startDate,
                EndDate = endDate,
                AutoRenew = autoRenew
            };

            bool success = await _budgetService.UpdateBudgetAsync(cmd);
            _console.WriteLine(success
                ? "Budget updated successfully."
                : "Failed to update budget.");
            _console.ReadKey();
        }

        public async Task DeleteBudgetAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Delete Budget ===");

            var budgets = (await _budgetService.GetAllBudgetsAsync()).ToList();
            if (!budgets.Any())
            {
                _console.WriteLine("No budgets available to delete.");
                _console.ReadKey();
                return;
            }

            foreach (var b in budgets)
            {
                _console.WriteLine($"ID: {b.Id} | Name: {b.Name} | Frequency: {b.Frequency} | Start: {b.StartDate:yyyy-MM-dd} | End: {b.EndDate:yyyy-MM-dd} | AutoRenew: {b.AutoRenew}");
            }

            Guid id;
            while (true)
            {
                id = _input.GetValidGuid("Enter the ID of the budget to delete: ");
                if (budgets.Any(b => b.Id == id)) break;
                _console.WriteLine("Invalid ID. Please select one from the list above.");
            }

            bool success = await _budgetService.DeleteBudgetAsync(id);
            _console.WriteLine(success ? "Budget deleted successfully." : "Failed to delete budget.");
            _console.ReadKey();
        }

    }
}
