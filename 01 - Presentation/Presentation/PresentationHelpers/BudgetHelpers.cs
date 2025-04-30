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

        public BudgetHelpers(
            IBudgetService budgetService,
            SelectBudgetContainer selector,
            InputProcessor input,
            IConsole console)
        {
            _budgetService = budgetService
                ?? throw new ArgumentNullException(nameof(budgetService));
            _selector = selector
                ?? throw new ArgumentNullException(nameof(selector));
            _input = input
                ?? throw new ArgumentNullException(nameof(input));
            _console = console
                ?? throw new ArgumentNullException(nameof(console));
        }

        public async Task CreateBudgetAsync()
        {
            _console.WriteLine("=== Create Budget ===");

            string name = _input.GetInput("Enter budget name: ");
            var frequency = _input.GetEnum("Enter frequency (Weekly/Monthly/Yearly): ",
                Domain.Entities.BudgetFrequency.Monthly);
            DateTime startDate = _input.GetValidDate("Enter start date (yyyy-MM-dd): ");
            DateTime endDate = _input.GetValidDate("Enter end date (yyyy-MM-dd): ");
            bool autoRenew = _input.GetBool("Auto renew? (y/n): ");

            int itemCount = _input.GetValidInt("Enter number of budget items: ");
            var items = new List<CreateBudgetItemCommand>();
            for (int i = 0; i < itemCount; i++)
            {
                _console.WriteLine($"--- Budget Item {i + 1} ---");
                string category = _input.GetInput("Enter category: ");
                decimal plannedAmount = _input.GetValidDecimal("Enter planned amount: ");

                items.Add(new CreateBudgetItemCommand
                {
                    Category = category,
                    PlannedAmount = plannedAmount
                });
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
            _console.WriteLine($"Budget '{dto.Name}' created with ID: {dto.Id}");
        }

        public async Task ViewBudgetsAsync()
        {
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
