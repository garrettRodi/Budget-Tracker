// File: Presentation/ReportingHelpers/SavingGoalsReportingHelpers.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public class SavingGoalsReportingHelpers
    {
        private readonly IReportingService _reportingService;
        private readonly SelectBudgetContainer _selector;
        private readonly IConsole _console;

        public SavingGoalsReportingHelpers(
            IReportingService reportingService,
            SelectBudgetContainer selector,
            IConsole console)
        {
            _reportingService = reportingService
                ?? throw new ArgumentNullException(nameof(reportingService));
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

            decimal totalSavings = 0m; // Track total for all saving goals + bulk

            foreach (var g in goals)
            {
                if (g.Id == Guid.Empty)
                {
                    _console.WriteLine($"Bulk/Uncategorized Savings:");
                    _console.WriteLine($"  Saved: {g.CurrentAmount:C}");
                    // Optionally: Don't show target/progress for bulk
                }
                else
                {
                    _console.WriteLine($"Goal: {g.GoalName}");
                    _console.WriteLine($"  Target Amount: {g.TargetAmount:C}");
                    _console.WriteLine($"  Current Saved: {g.CurrentAmount:C}");
                    var progress = g.TargetAmount.Amount > 0
                        ? g.CurrentAmount.Amount / g.TargetAmount.Amount * 100
                        : 0;
                    _console.WriteLine($"  Progress: {progress:F2}%");
                }
                _console.WriteLine(new string('-', 40));
                totalSavings += g.CurrentAmount.Amount;
            }
            _console.WriteLine($"Total Savings (All Goals + Bulk): {totalSavings:C}");
            _console.ReadKey();
        }
    }
}
