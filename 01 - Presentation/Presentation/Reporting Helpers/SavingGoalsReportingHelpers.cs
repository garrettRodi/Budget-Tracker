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
            _console.WriteLine("=== Saving Goals Report ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var goals = await _reportingService.GenerateSavingGoalReportAsync(budgetId);
            if (!goals.Any())
            {
                _console.WriteLine("No saving goals found for the active budget.");
                return;
            }

            foreach (var g in goals)
            {
                _console.WriteLine($"Goal: {g.GoalName}");
                _console.WriteLine($"  Target Amount: {g.TargetAmount:C}");
                _console.WriteLine($"  Current Saved: {g.CurrentAmount:C}");
                var progress = g.TargetAmount > 0
                    ? g.CurrentAmount / g.TargetAmount * 100
                    : 0;
                _console.WriteLine($"  Progress: {progress:F2}%");
                _console.WriteLine(new string('-', 40));
            }
        }
    }
}
