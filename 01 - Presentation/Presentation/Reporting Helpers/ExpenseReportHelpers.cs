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
            _console.ReadKey();
        }
    }
}
