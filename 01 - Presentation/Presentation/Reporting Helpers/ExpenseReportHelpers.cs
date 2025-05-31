// File: Presentation/ReportingHelpers/ExpenseReportHelpers.cs
using System;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public class ExpenseReportHelpers
    {
        private readonly IReportingService _reportingService;
        private readonly SelectBudgetContainer _selector;
        private readonly InputProcessor _input;
        private readonly IConsole _console;

        public ExpenseReportHelpers(
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

        public async Task ViewExpenseReportAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Detailed Expense Report ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var start = _input.GetValidDate("Enter start date (yyyy-MM-dd): ");
            var end = _input.GetValidDate("Enter end date (yyyy-MM-dd): ");

            var report = await _reportingService.GenerateExpenseReportAsync(budgetId, start, end);

            _console.WriteLine($"Expense Report ({start:yyyy-MM-dd} – {end:yyyy-MM-dd})");
            _console.WriteLine($"Total Expenses: {report.TotalExpenses:C}");
            _console.WriteLine("Category Breakdown:");
            foreach (var kvp in report.CategoryTotals)
            {
                var category = kvp.Key;
                var total = kvp.Value;
                var percent = report.CategoryPercentages[category];
                _console.WriteLine($"  {category}: {total:C} ({percent:F2}%)");
            }

            // If "Savings" is present, show savings breakdown
            if (report.CategoryTotals.ContainsKey("Savings"))
            {
                _console.WriteLine("\nSavings Breakdown:");
                var goals = (await _reportingService.GenerateSavingGoalReportAsync(budgetId)).ToList();
                var bulk = goals.FirstOrDefault(g => g.Id == Guid.Empty);
                var goalTotal = goals.Where(g => g.Id != Guid.Empty).Sum(g => g.CurrentAmount.Amount);

                foreach (var g in goals.Where(g => g.Id != Guid.Empty))
                    _console.WriteLine($"  {g.GoalName}: {g.CurrentAmount:C}");

                if (bulk != null && bulk.CurrentAmount.Amount > 0)
                    _console.WriteLine($"  Bulk/Uncategorized: {bulk.CurrentAmount:C}");

                _console.WriteLine($"  === Total Savings: {(goalTotal + (bulk?.CurrentAmount.Amount ?? 0)):C}");
            }
            _console.ReadKey();
        }
    }
}
