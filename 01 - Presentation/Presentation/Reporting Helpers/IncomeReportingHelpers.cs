// File: Presentation/ReportingHelpers/IncomeReportingHelpers.cs
using System;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public class IncomeReportingHelpers
    {
        private readonly IReportingService _reportingService;
        private readonly SelectBudgetContainer _selector;
        private readonly InputProcessor _input;
        private readonly IConsole _console;

        public IncomeReportingHelpers(
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

        public async Task ViewIncomeReportAsync()
        {
            _console.WriteLine("=== Income Report ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var start = _input.GetValidDate("Enter start date (yyyy-MM-dd): ");
            var end = _input.GetValidDate("Enter end date (yyyy-MM-dd): ");

            var report = await _reportingService.GenerateIncomeReportAsync(budgetId, start, end);

            _console.WriteLine($"Income Report ({start:yyyy-MM-dd} – {end:yyyy-MM-dd})");
            _console.WriteLine($"Total Income: {report.TotalIncome:C}");
        }
    }
}
