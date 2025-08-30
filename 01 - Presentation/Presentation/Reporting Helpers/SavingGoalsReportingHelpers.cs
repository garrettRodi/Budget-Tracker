// File: Presentation/ReportingHelpers/SavingGoalsReportingHelpers.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Presentation.PresentationHelpers;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public class SavingGoalsReportingHelpers
    {
        private readonly IReportingService _reportingService;
        private readonly ICurrencyService _currencyService;
        private readonly SelectBudgetContainer _selector;
        private readonly IConsole _console;

        public SavingGoalsReportingHelpers(
            IReportingService reportingService,
            ICurrencyService currencyService,
            SelectBudgetContainer selector,
            IConsole console)
        {
            _reportingService = reportingService
                ?? throw new ArgumentNullException(nameof(reportingService));
            _currencyService = currencyService
                ?? throw new ArgumentNullException(nameof(currencyService));
            _selector = selector
                ?? throw new ArgumentNullException(nameof(selector));
            _console = console
                ?? throw new ArgumentNullException(nameof(console));
        }

        public async Task ViewSavingGoalsReportAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Saving Goals Report ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var goals = (await _reportingService.GenerateSavingGoalReportAsync(budgetId)).ToList();
            if (!goals.Any())
            {
                _console.WriteLine("No saving goals found for the active budget.");
                return;
            }
            // 1. Initialize total savings in the current currency
            var totalSavings = new Money(0m, _currencyService.CurrentCurrency);

            foreach (var g in goals)
            {
                var convertedCurrentAmount = await _currencyService.ConvertAsync(g.CurrentAmount);
                totalSavings += convertedCurrentAmount;

                if (g.Id == Guid.Empty)
                {
                    _console.WriteLine("Bulk/Uncategorized Savings:");
                    _console.WriteLine($"  Saved: {await convertedCurrentAmount.ToDisplayAsync(_currencyService)}");
                }
                else
                {
                    // 2. For each goal, convert the target amounts to the current currency
                    var convertedTargetAmount = await _currencyService.ConvertAsync(g.TargetAmount);

                    _console.WriteLine($"Goal: {g.GoalName}");
                    _console.WriteLine($"  Target Amount: {await convertedTargetAmount.ToDisplayAsync(_currencyService)}");
                    _console.WriteLine($"  Current Saved: {await convertedCurrentAmount.ToDisplayAsync(_currencyService)}");

                    // 3. Calculate and display the progress
                    var progress = g.TargetAmount.Amount > 0
                        ? g.CurrentAmount.Amount / g.TargetAmount.Amount * 100m
                        : 0m;
                    _console.WriteLine($"  Progress: {progress:F2}%");
                }
                _console.WriteLine(new string('-', 40));
            }
            // 4. Display the total savings across all goals
            _console.WriteLine($"Total Savings (All Goals + Bulk): {await totalSavings.ToDisplayAsync(_currencyService)}");
            _console.ReadKey();
        }
    }
}
