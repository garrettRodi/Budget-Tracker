// File: Presentation/UIHelpers/SelectBudgetContainer.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.Interfaces;

namespace BudgetTracker.Presentation.UIHelpers
{
    public class SelectBudgetContainer
    {
        private readonly IBudgetService _budgetService;
        private readonly IConsole _console;

        public SelectBudgetContainer(IBudgetService budgetService, IConsole console)
        {
            _budgetService = budgetService ?? throw new ArgumentNullException(nameof(budgetService));
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        /// <summary>
        /// Prompts the user to choose an active budget container, or returns Guid.Empty if none exist.
        /// </summary>
        public async Task<Guid> GetActiveBudgetContainerIdAsync()
        {
            var budgets = (await _budgetService.GetAllBudgetsAsync()).ToList();
            if (budgets.Count == 0)
            {
                _console.WriteLine("No budgets found. Please create a budget first.");
                return Guid.Empty;
            }
            if (budgets.Count == 1)
            {
                _console.WriteLine($"Using the only budget: {budgets[0].Name}");
                return budgets[0].Id;
            }

            _console.WriteLine("Multiple budgets found. Please select one:");
            for (int i = 0; i < budgets.Count; i++)
            {
                var b = budgets[i];
                _console.WriteLine($"{i + 1}. {b.Name} (ID: {b.Id})");
            }

            while (true)
            {
                _console.Write("Enter the number of your choice: ");
                var input = _console.ReadLine();
                if (int.TryParse(input, out var idx) && idx >= 1 && idx <= budgets.Count)
                {
                    return budgets[idx - 1].Id;
                }
                _console.WriteLine("Invalid selection. Try again.");
            }
        }
    }
}
