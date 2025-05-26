using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.PresentationHelpers
{
    public class PlannedIncomeHelpers
    {
        private readonly IPlannedIncomeService _plannedService;
        private readonly SelectBudgetContainer _selector;
        private readonly InputProcessor _input;
        private readonly IConsole _console;

        public PlannedIncomeHelpers(
            IPlannedIncomeService plannedService,
            SelectBudgetContainer selector,
            InputProcessor input,
            IConsole console)
        {
            _plannedService = plannedService;
            _selector = selector;
            _input = input;
            _console = console;
        }

        public async Task CreatePlannedIncomeAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Create Planned Income ===");

            Guid budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            DateTime period = _input.GetValidDate("Enter period date (yyyy-MM-dd): ");
            decimal amount = _input.GetValidDecimal("Enter planned amount: ");

            var cmd = new CreatePlannedIncomeCommand
            {
                BudgetContainerId = budgetId,
                PeriodStart = period,
                Amount = amount
            };

            var dto = await _plannedService.CreatePlannedIncomeAsync(cmd);
            _console.WriteLine($"Planned income for {dto.PeriodStart:d} created (ID: {dto.Id})");
            _console.ReadKey();
        }

        public async Task ViewPlannedIncomesAsync()
        {
            _console.Clear();
            _console.WriteLine("=== View Planned Incomes ===");

            Guid budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var list = await _plannedService.GetPlannedIncomesByBudgetAsync(budgetId);
            foreach (var pi in list)
            {
                _console.WriteLine($"ID: {pi.Id} | Date: {pi.PeriodStart:d} | Amount: {pi.Amount:C}");
            }
            if (!list.Any())
                _console.WriteLine("No planned incomes found.");
            _console.ReadKey();
        }

        public async Task UpdatePlannedIncomeAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Update Planned Income ===");

            Guid budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            _console.Write("Enter planned income ID: ");
            if (!Guid.TryParse(_console.ReadLine(), out var id))
            {
                _console.WriteLine("Invalid ID format."); return;
            }

            DateTime period = _input.GetValidDate("Enter new period date (yyyy-MM-dd): ");
            decimal amount = _input.GetValidDecimal("Enter new planned amount: ");

            var cmd = new UpdatePlannedIncomeCommand
            {
                Id = id,
                PeriodStart = period,
                Amount = amount
            };

            bool ok = await _plannedService.UpdatePlannedIncomeAsync(cmd);
            _console.WriteLine(ok ? "Updated successfully." : "Update failed.");
            _console.ReadKey();
        }

        public async Task DeletePlannedIncomeAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Delete Planned Income ===");

            Guid budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            _console.Write("Enter planned income ID: ");
            if (!Guid.TryParse(_console.ReadLine(), out var id))
            {
                _console.WriteLine("Invalid ID format."); return;
            }

            bool ok = await _plannedService.DeletePlannedIncomeAsync(id);
            _console.WriteLine(ok ? "Deleted successfully." : "Delete failed.");
            _console.ReadKey();
        }
    }
}
