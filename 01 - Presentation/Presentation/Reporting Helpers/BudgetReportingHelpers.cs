// File: Presentation/ReportingHelpers/BudgetReportingHelpers.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Services;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Presentation.UIHelpers;
using Microsoft.VisualBasic;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public class BudgetReportingHelpers
    {
        private readonly IReportingService _reportingService;
        private readonly SelectBudgetContainer _selector;
        private readonly IBudgetService _budgetService;
        private readonly InputProcessor _input;
        private readonly IConsole _console;
        private readonly ICurrencyService _currencyService;

        public BudgetReportingHelpers(
            IReportingService reportingService,
            SelectBudgetContainer selector,
            IBudgetService budgetService,
            InputProcessor input,
            IConsole console,
            ICurrencyService currencyService)
        {
            _reportingService = reportingService
                ?? throw new ArgumentNullException(nameof(reportingService));
            _selector = selector
                ?? throw new ArgumentNullException(nameof(selector));
            _budgetService = budgetService
            ?? throw new ArgumentNullException(nameof(budgetService));
            _input = input
                ?? throw new ArgumentNullException(nameof(input));
            _console = console
                ?? throw new ArgumentNullException(nameof(console));
            _currencyService = currencyService
                ?? throw new ArgumentNullException(nameof(currencyService));
        }

        /// <summary>
        ///  View a 50/20/30 or other budget rule report.
        /// </summary>
        public async Task ViewBudgetRuleReportAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Budget Rule Report ===");
            Guid budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            var rule = _input.GetInput("Enter budget rule (e.g., 50/20/30): ");
            var start = _input.GetValidDate("Enter start date (yyyy-MM-dd): ");
            var end = _input.GetValidDate("Enter end date (yyyy-MM-dd): ");

            var report = await _reportingService.GenerateBudgetRuleReportAsync(rule, budgetId, start, end);

            _console.WriteLine($"Rule: {report.Rule}");
            _console.WriteLine($"  Necessities - Planned: {report.NecessitiesPlanned:C}, Actual: {report.NecessitiesActual:C}, Variance: {report.NecessitiesPercentageVariance:F2}%");

            // Add uncategorized/ bulk savings to the Savings line
            var savingGoalsReport = (await _reportingService.GenerateSavingGoalReportAsync(budgetId)).ToList();
            var bulk = savingGoalsReport.FirstOrDefault(g => g.Id == Guid.Empty);
            Money bulkAmount = bulk?.CurrentAmount ?? new Money(0m, _currencyService.CurrentCurrency); ;

            Money totalSavingsActual = report.SavingsActual + bulkAmount;
            _console.WriteLine($"  Savings      - Planned: {report.SavingsPlanned:C}, Actual: {totalSavingsActual:C} (Goals: {report.SavingsActual:C} + Bulk: {bulkAmount:C}), Variance: {report.SavingsPercentageVariance:F2}%");
            _console.WriteLine($"  Discretion.  - Planned: {report.DiscretionaryPlanned:C}, Actual: {report.DiscretionaryActual:C}, Variance: {report.DiscretionaryPercentageVariance:F2}%");
            _console.ReadKey();
        }
        public async Task ViewBudgetMatrixReportAsync()
        {
            // 1) Get the active budget and its matrix
            Guid budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            var matrix = await _reportingService.GenerateBudgetMatrixReportAsync(budgetId);

            // 2) Use exactly the periods your service populated
            var periods = matrix.ReportingPeriods;
            var categories = matrix.Categories;

            // 3) Build pages (7 days per page for daily frequencies)
            const int PageSize = 7;
            var pages = periods
                .Select((date, idx) => new { date, idx })
                .GroupBy(x => x.idx / PageSize)
                .Select(g => g.Select(x => x.date).ToList())
                .ToList();

            // Save original console color to restore later
            var prevColor = Console.ForegroundColor;

            // 4) Render each page
            for (int p = 0; p < pages.Count; p++)
            {
                var pageDates = pages[p];

                Console.Clear();
                Console.WriteLine(
                    $"=== Budget Matrix: {matrix.StartDate:MM/dd/yyyy} – {matrix.EndDate:MM/dd/yyyy} " +
                    $"(Page {p + 1}/{pages.Count}) ===\n");

                // --- Build separator line dynamically ---
                string sep = "+" + new string('-', 15) + "+";
                foreach (var _ in pageDates) sep += new string('-', 13) + "+";
                sep += new string('-', 11) + "+";
                Console.WriteLine(sep);

                // --- Header row ---
                Console.Write("| Category".PadRight(15) + "|");
                foreach (var d in pageDates)
                    Console.Write(d.ToString("MM/dd").PadLeft(12) + "|");
                Console.Write(" TotPln".PadLeft(11) + "|\n");

                Console.WriteLine(sep.Replace('-', '='));

                // --- Data rows per category ---
                foreach (var cat in categories)
                {
                    Console.Write("| " + cat.PadRight(13) + "|");

                    Money rowPln = new Money(0, _currencyService.CurrentCurrency);
                    Money rowAct = new Money(0, _currencyService.CurrentCurrency);
                    foreach (var d in pageDates)
                    {
                        var pln = matrix.PlannedByCategoryAndDate[(cat, d)];
                        var act = matrix.ActualByCategoryAndDate[(cat, d)];
                        rowPln += pln;
                        rowAct += act;
                        Console.Write($"{pln,6:C}/{act,-6:C}|");
                    }

                    Console.Write($"{rowPln,9:C}|\n");
                    Console.WriteLine(sep);
                }

                // --- Differences Row for this page ---
                Console.Write("| " + "Diff".PadRight(13) + "|");
                Money pageTotalDiff = new Money(0, _currencyService.CurrentCurrency);
                foreach (var d in pageDates)
                {
                    Money periodPln = categories
                        .Select(cat => matrix.PlannedByCategoryAndDate[(cat, d)])
                        .Aggregate(new Money(0m, _currencyService.CurrentCurrency), (sum, m) => sum + m);
                    Money periodAct = categories
                        .Select(cat => matrix.ActualByCategoryAndDate[(cat, d)])
                        .Aggregate(new Money(0m, _currencyService.CurrentCurrency), (sum, m) => sum + m);
                    Money periodDiff = periodPln - periodAct;
                    pageTotalDiff += periodDiff;

                    Console.ForegroundColor = periodDiff.Amount >= 0
                        ? ConsoleColor.Green
                        : ConsoleColor.Red;
                    Console.Write($"{periodDiff,12:C}|");
                    Console.ForegroundColor = prevColor;
                }

                // Page total difference column
                Console.ForegroundColor = pageTotalDiff.Amount >= 0
                    ? ConsoleColor.Green
                    : ConsoleColor.Red;
                Console.Write($"{pageTotalDiff,11:C}|\n");
                Console.ForegroundColor = prevColor;

                Console.WriteLine(sep);

                // Pause between pages
                if (pages.Count > 1 && p < pages.Count - 1)
                {
                    Console.Write("Press any key for next page...");
                    Console.ReadKey(true);
                }
            }

            // 5) Final totals & averages for Yearly budgets
            _console.WriteLine($"DEBUG: _budgetService is {(_budgetService == null ? "NULL" : "OK")}");
            _console.WriteLine($"DEBUG: _reportingService is {(_reportingService == null ? "NULL" : "OK")}");
            _console.WriteLine($"DEBUG: matrix is {(matrix == null ? "NULL" : "OK")}");
            _console.WriteLine($"DEBUG: matrix.ReportingPeriods is {(matrix?.ReportingPeriods == null ? "NULL" : "OK")}");
            _console.WriteLine($"DEBUG: matrix.Categories is {(matrix?.Categories == null ? "NULL" : "OK")}");
            var budget = await _budgetService.GetBudgetByIdAsync(budgetId);
            if (budget != null && budget.Frequency == BudgetFrequency.Yearly)
            {
                int divisor = budget.Frequency switch
                {
                    BudgetFrequency.Weekly => 7,
                    BudgetFrequency.Monthly => (matrix.EndDate - matrix.StartDate).Days + 1,
                    BudgetFrequency.Yearly => periods.Count,
                    _ => periods.Count
                };

                _console.WriteLine("\n=== Yearly Category Totals ===");
                foreach (var cat in categories)
                {
                    var currency = matrix.PlannedByCategoryAndDate[(cat, periods.First())].Currency; // or get from context/service
                    Money totPln = periods
                        .Select(d => matrix.PlannedByCategoryAndDate[(cat, d)])
                        .Aggregate(new Money(0m, currency), (sum, m) => sum + m);

                    Money totAct = periods
                        .Select(d => matrix.ActualByCategoryAndDate[(cat, d)])
                        .Aggregate(new Money(0m, currency), (sum, m) => sum + m);

                    Money totDiff = totPln - totAct;
                    bool isSavings = cat.Equals("Savings", StringComparison.OrdinalIgnoreCase);

                    Console.ForegroundColor = (totDiff.Amount >= 0) ^ isSavings
                        ? ConsoleColor.Green
                        : ConsoleColor.Red;
                    _console.WriteLine($"{cat.PadRight(15)} {totDiff,12:C}");
                    Console.ForegroundColor = prevColor;
                }

                Money grandTotalDiff = categories
                    .Select(cat => periods
                    .Select(d => matrix.PlannedByCategoryAndDate[(cat, d)] - matrix.ActualByCategoryAndDate[(cat, d)])
                    .Aggregate(new Money(0m, _currencyService.CurrentCurrency), (sum, m) => sum + m))
                    .Aggregate(new Money(0m, _currencyService.CurrentCurrency), (sum, m) => sum + m);
                Money avgDiff = grandTotalDiff/ divisor;

                _console.WriteLine($"\nAverage Difference ({divisor} periods): {avgDiff,12:C}");
            }

            //  Show bulk/uncategorized savings after the matrix 
            var savingGoalsReport = (await _reportingService.GenerateSavingGoalReportAsync(budgetId)).ToList();
            var bulk = savingGoalsReport.FirstOrDefault(g => g.Id == Guid.Empty);
            if (bulk != null && bulk.CurrentAmount.Amount > 0)
            {
                _console.WriteLine($"\nBulk/Uncategorized Savings: {bulk.CurrentAmount.Amount:C}");
            }

            _console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey(true);
        }
    }
}
