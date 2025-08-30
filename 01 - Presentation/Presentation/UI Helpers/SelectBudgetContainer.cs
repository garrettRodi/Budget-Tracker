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
        private readonly ICurrencyService _currencyService;

        public SelectBudgetContainer(
            IBudgetService budgetService,
            IConsole console,
            ICurrencyService currencyService)
        {
            _budgetService = budgetService ?? throw new ArgumentNullException(nameof(budgetService));
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
        }
        public async Task<Guid> GetActiveBudgetContainerIdAsync()
        {
            var budgets = (await _budgetService.GetAllBudgetsAsync()).ToList();
            if (!budgets.Any())
            {
                _console.WriteLine("No budgets found. Please create one first.");
                return Guid.Empty;
            }
            BudgetDTO chosen;
            if (budgets.Count == 1)
            {
                chosen = budgets[0];
                _console.WriteLine($"Using the only budget: {chosen.Name}");
            }
            else
            {
                _console.WriteLine("Multiple budgets found. Please select one:");
                for (int i = 0; i < budgets.Count; i++)
                {
                    _console.WriteLine($"{i + 1}. {budgets[i].Name} (Currency: {budgets[i].Currency})");
                }

                int idx;
                do
                {
                    _console.Write("Enter the number of your choice: ");
                }
                while (!int.TryParse(_console.ReadLine(), out idx)
                       || idx < 1
                       || idx > budgets.Count);

                chosen = budgets[idx - 1];
            }

            return chosen.Id;
        }
    }
}
