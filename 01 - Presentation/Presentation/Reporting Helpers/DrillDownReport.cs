// File: Presentation/ReportingHelpers/DrillDownReport.cs
using System;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public class DrillDownReport
    {
        private readonly IReportingService _reportingService;
        private readonly SelectBudgetContainer _selector;
        private readonly InputProcessor _input;
        private readonly IConsole _console;

        public DrillDownReport(
            IReportingService reportingService,
            SelectBudgetContainer selector,
            InputProcessor input,
            IConsole console)
        {
            _reportingService = reportingService
                ?? throw new ArgumentNullException(nameof(reportingService));
            _selector = selector
                ?? throw new ArgumentNullException(nameof(selector));
            _input = input
                ?? throw new ArgumentNullException(nameof(input));
            _console = console
                ?? throw new ArgumentNullException(nameof(console));
        }

        public async Task ViewDrillDownReportAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Drill-Down Expense Report ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var category = _input.GetTitleInput("Enter the expense category to drill down (e.g., Food): ");
            var start = _input.GetValidDate("Enter start date (yyyy-MM-dd): ");
            var end = _input.GetValidDate("Enter end date (yyyy-MM-dd): ");

            var report = await _reportingService.GetFilteredExpensesAsync(budgetId, category, start, end);

            if (!report.CategoryTotals.TryGetValue(category, out var total))
            {
                _console.WriteLine($"No expenses found for category '{category}' in the given period.");
            }
            else
            {
                var percent = report.CategoryPercentages[category];
                _console.WriteLine($"Drill-Down Report for Category: {category}");
                _console.WriteLine($"  Total Expenses: {total:C}");
                _console.WriteLine($"  Percentage of Total: {percent:F2}%");
            }

            // For 'Savings', inform about bulk/uncategorized breakdown if present
            if (category.Equals("Savings", StringComparison.OrdinalIgnoreCase))
            {
                var goals = (await _reportingService.GenerateSavingGoalReportAsync(budgetId)).ToList();
                var bulk = goals.FirstOrDefault(g => g.Id == Guid.Empty);
                var goalTotal = goals.Where(g => g.Id != Guid.Empty).Sum(g => g.CurrentAmount);

                _console.WriteLine();
                _console.WriteLine("Savings Breakdown:");
                foreach (var g in goals.Where(g => g.Id != Guid.Empty))
                    _console.WriteLine($"  {g.GoalName}: {g.CurrentAmount:C}");

                if (bulk != null && bulk.CurrentAmount > 0)
                    _console.WriteLine($"  Bulk/Uncategorized: {bulk.CurrentAmount:C}");

                _console.WriteLine($"  === Total Savings: {(goalTotal + (bulk?.CurrentAmount ?? 0)):C}");
            }
            _console.ReadKey();
        }
    }
}
