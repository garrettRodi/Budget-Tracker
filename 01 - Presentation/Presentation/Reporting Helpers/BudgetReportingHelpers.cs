// File: Presentation/ReportingHelpers/BudgetReportingHelpers.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public class BudgetReportingHelpers
    {
        private readonly IReportingService _reportingService;
        private readonly SelectBudgetContainer _selector;
        private readonly InputProcessor _input;
        private readonly IConsole _console;

        public BudgetReportingHelpers(
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

        /// <summary>
        ///  View a 50/20/30 or other budget rule report.
        /// </summary>
        public async Task ViewBudgetRuleReportAsync()
        {
            _console.WriteLine("=== Budget Rule Report ===");
            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var rule = _input.GetInput("Enter budget rule (e.g., 50/20/30): ");
            var start = _input.GetValidDate("Enter start date (yyyy-MM-dd): ");
            var end = _input.GetValidDate("Enter end date (yyyy-MM-dd): ");

            var report = await _reportingService.GenerateBudgetRuleReportAsync(rule, budgetId, start, end);

            _console.WriteLine($"Rule: {report.Rule}");
            _console.WriteLine($"  Necessities - Planned: {report.NecessitiesPlanned:C}, Actual: {report.NecessitiesActual:C}, Variance: {report.NecessitiesPercentageVariance:F2}%");
            _console.WriteLine($"  Savings      - Planned: {report.SavingsPlanned:C}, Actual: {report.SavingsActual:C}, Variance: {report.SavingsPercentageVariance:F2}%");
            _console.WriteLine($"  Discretion.  - Planned: {report.DiscretionaryPlanned:C}, Actual: {report.DiscretionaryActual:C}, Variance: {report.DiscretionaryPercentageVariance:F2}%");
        }

        /// <summary>
        ///  View the detailed budget matrix over the budget period.
        /// </summary>
        public async Task ViewBudgetMatrixReportAsync()
        {
            _console.WriteLine("=== Budget Matrix Report ===");
            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var matrix = await _reportingService.GenerateBudgetMatrixReportAsync(budgetId);

            // Build pages of up to 7 columns
            var dates = matrix.ReportingPeriods.ToList();
            const int PageSize = 7;
            var pages = dates
                .Select((date, idx) => new { date, idx })
                .GroupBy(x => x.idx / PageSize)
                .Select(g => g.Select(x => x.date).ToList())
                .ToList();

            for (int p = 0; p < pages.Count; p++)
            {
                var pageDates = pages[p];
                _console.WriteLine($"\nPage {p + 1}/{pages.Count}: {pageDates.First():MM/dd/yyyy} – {pageDates.Last():MM/dd/yyyy}");

                // Header row
                var header = "Category".PadRight(15) + "|";
                foreach (var d in pageDates)
                    header += d.ToString("MM/dd").PadLeft(13) + "|";
                header += " TotPln".PadLeft(11) + "|" + " TotAct".PadLeft(11) + "|" + " Diff".PadLeft(9);
                _console.WriteLine(header);

                // Rows
                foreach (var cat in matrix.Categories)
                {
                    decimal rowPln = 0, rowAct = 0;
                    var line = cat.PadRight(15) + "|";
                    foreach (var d in pageDates)
                    {
                        matrix.PlannedByCategoryAndDate.TryGetValue((cat, d), out var pln);
                        matrix.ActualByCategoryAndDate.TryGetValue((cat, d), out var act);
                        rowPln += pln; rowAct += act;
                        line += $"{pln,6:C}/{act,-6:C}|";
                    }
                    var diff = rowPln - rowAct;
                    line += $"{rowPln,9:C}|{rowAct,11:C}|{diff,9:C}";
                    _console.WriteLine(line);
                }

                if (pages.Count > 1 && p < pages.Count - 1)
                {
                    _console.Write("Press Enter for next page...");
                    _console.ReadLine();
                }
            }
        }
    }
}
