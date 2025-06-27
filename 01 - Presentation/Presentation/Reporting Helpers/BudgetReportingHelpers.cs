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

            _console.Clear();
            // 1) Get data
            Guid budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            var matrix = await _reportingService.GenerateBudgetMatrixReportAsync(budgetId);

            _console.WriteLine("DEBUG: Categories = " + string.Join(", ", matrix.Categories));
            var plannedKeys = matrix.PlannedByCategoryAndDate
                     .Keys
                     .Select(k => k.Item1)
                     .Distinct();
            _console.WriteLine("DEBUG: Planned keys = " + string.Join(", ", plannedKeys));
_console.ReadKey();
            var periods = matrix.ReportingPeriods;
            var categories = matrix.Categories;

            // 2) Split into pages of up to 7 periods
            const int PageSize = 7;
            var pages = periods
                .Select((date, idx) => new { date, idx })
                .GroupBy(x => x.idx / PageSize)
                .Select(g => g.Select(x => x.date).ToList())
                .ToList();

            // 3) Column widths and colors
            const int categoryColWidth = 15;
            const int cellColWidth = 15; // fits "1,234.00/1,234.00"
            var defaultCurrency = _currencyService.CurrentCurrency;
            var prevColor = _console.ForegroundColor;
            int currentPage = 0;
            int totalPages = pages.Count + 1; // +1 for the summary page


            // 4) Paging loop
            while (true)
            {
                _console.Clear();

                if (currentPage < pages.Count)
                {
                    var pageDates = pages[currentPage];

                    // Title and legend
                    _console.WriteLine(
                        $"=== Budget Matrix: {matrix.StartDate:MM/dd/yyyy} – {matrix.EndDate:MM/dd/yyyy} " +
                        $"(Page {currentPage + 1}/{totalPages}) ===");
                    _console.WriteLine("(Each cell: Planned / Actual)");

                    // Separator line
                    string sep = "+" + new string('-', categoryColWidth) + "+";
                    foreach (var _ in pageDates) sep += new string('-', cellColWidth) + "+";
                    _console.WriteLine(sep);

                    // Header row
                    _console.Write("| Category".PadRight(categoryColWidth) + "|");
                    foreach (var d in pageDates)
                        _console.Write(d.ToString("MM/yyyy").PadLeft(cellColWidth) + "|");
                    _console.WriteLine();
                    _console.WriteLine(sep.Replace('-', '='));

                    // Income row (use TryGetValue and fall back to zero)
                    _console.Write("| Income".PadRight(categoryColWidth) + "|");
                    foreach (var d in pageDates)
                    {
                        matrix.PlannedByCategoryAndDate.TryGetValue(("Income", d), out var incomePln);
                        matrix.ActualByCategoryAndDate.TryGetValue(("Income", d), out var incomeAct);
                        incomePln ??= new Money(0m, defaultCurrency);
                        incomeAct ??= new Money(0m, defaultCurrency);
                        string incomeCell = $"{incomePln.Amount:C}/{incomeAct.Amount:C}".PadLeft(cellColWidth);
                        _console.Write(incomeCell + "|");
                    }
                    _console.WriteLine();
                    _console.WriteLine(sep);

                    // Data rows for each category
                    foreach (var cat in categories)
                    {
                        _console.Write("| " + cat.PadRight(categoryColWidth - 2) + " |");
                        foreach (var d in pageDates)
                        {
                            var pln = matrix.PlannedByCategoryAndDate[(cat, d)];
                            var act = matrix.ActualByCategoryAndDate[(cat, d)];
                            string cell = $"{pln.Amount:C}/{act.Amount:C}".PadLeft(cellColWidth);
                            _console.Write(cell + "|");
                        }
                        _console.WriteLine();
                        _console.WriteLine(sep);
                    }

                    // Period Totals row: income minus sum of other categories
                    _console.Write("| Period Totals".PadRight(categoryColWidth) + "|");
                    foreach (var d in pageDates)
                    {
                        matrix.PlannedByCategoryAndDate.TryGetValue(("Income", d), out var incomePln);
                        matrix.ActualByCategoryAndDate.TryGetValue(("Income", d), out var incomeAct);
                        incomePln ??= new Money(0m, defaultCurrency);
                        incomeAct ??= new Money(0m, defaultCurrency);

                        var expPln = categories
                            .Select(c => matrix.PlannedByCategoryAndDate[(c, d)])
                            .Aggregate(new Money(0m, defaultCurrency), (sum, m) => sum + m);
                        var expAct = categories
                            .Select(c => matrix.ActualByCategoryAndDate[(c, d)])
                            .Aggregate(new Money(0m, defaultCurrency), (sum, m) => sum + m);

                        var totPln = incomePln - expPln;
                        var totAct = incomeAct - expAct;
                        // Prepare the two parts
                        var plnStr = totPln.Amount.ToString("C");
                        var actStr = totAct.Amount.ToString("C");

                        // Right-align within the column
                        int padding = cellColWidth - (plnStr.Length + 1 + actStr.Length);
                        _console.Write(new string(' ', Math.Max(padding, 0)));

                        // 1) Color & write the Planned‐surplus portion
                        _console.ForegroundColor = totPln.Amount >= 0
                            ? ConsoleColor.Green   // good: planned income ≥ planned expenses
                            : ConsoleColor.Red;     // bad: planned expenses > planned income
                        _console.Write(plnStr);
                        _console.ForegroundColor = prevColor; // reset

                        _console.Write("/"); // separator (neutral)

                        // 2) Color & write the Actual‐surplus portion
                        _console.ForegroundColor = totAct.Amount >= 0
                            ? ConsoleColor.Green   // good: actual income ≥ actual expenses
                            : ConsoleColor.Red;     // bad: actual expenses > actual income
                        _console.Write(actStr);
                        _console.ForegroundColor = prevColor; // reset

                        _console.Write("|"); // end cell
                        _console.ForegroundColor = prevColor;
                    }
                    _console.WriteLine();
                    _console.WriteLine(sep);
                }

                else
                {
                    // --- Summary Page (last page) ---
                    _console.WriteLine($"=== Summary: {matrix.StartDate:MM/dd/yyyy} – {matrix.EndDate:MM/dd/yyyy} (Page {currentPage + 1}/{totalPages}) ===");
                    _console.WriteLine("(Planned / Actual → Difference / Avg Diff)");

                    // Separator
                    string sep = "+" + new string('-', categoryColWidth) + "+"
                               + new string('-', cellColWidth * 4 + 3) + "+";
                    _console.WriteLine(sep.Replace('-', '='));

                    // Header
                    _console.Write("| Category".PadRight(categoryColWidth) + "|");
                    _console.Write("Total Planned".PadLeft(cellColWidth) + "|");
                    _console.Write("Total Actual".PadLeft(cellColWidth) + "|");
                    _console.Write("Total Diff".PadLeft(cellColWidth) + "|");
                    _console.Write("Avg Diff".PadLeft(cellColWidth) + "|\n");
                    _console.WriteLine(sep);

                    // For each category (including Income)
                    var allCats = new[] { "Income" }.Concat(categories);
                    foreach (var cat in allCats)
                    {
                        _console.Write("| " + cat.PadRight(categoryColWidth - 2) + " |");

                        // Totals
                        var totalPln = periods
                            .Select(d => matrix.PlannedByCategoryAndDate.TryGetValue((cat, d), out var p) ? p : new Money(0m, defaultCurrency))
                            .Aggregate(new Money(0m, defaultCurrency), (s, m) => s + m);
                        var totalAct = periods
                            .Select(d => matrix.ActualByCategoryAndDate.TryGetValue((cat, d), out var a) ? a : new Money(0m, defaultCurrency))
                            .Aggregate(new Money(0m, defaultCurrency), (s, m) => s + m);


                        // Write cells
                        _console.Write(totalPln.Amount.ToString("C").PadLeft(cellColWidth) + "|");
                        _console.Write(totalAct.Amount.ToString("C").PadLeft(cellColWidth) + "|");

                        // Difference
                        var diff = totalAct - totalPln;

                        
                        // Color logic:
                        //  • Income & Savings: diff >= 0 → good (green); diff < 0 → bad (red)
                        //  • Other categories (expenses): diff <= 0 → good (green); diff > 0 → bad (red)
                        bool isIncome = cat.Equals("Income", StringComparison.OrdinalIgnoreCase);
                        bool isSavings = cat.Equals("Savings", StringComparison.OrdinalIgnoreCase);
                        if (isIncome || isSavings)
                        {
                            _console.ForegroundColor = diff.Amount >= 0
                        ? ConsoleColor.Green
                        : ConsoleColor.Red;
                        }
                        else
                        {
                            _console.ForegroundColor = diff.Amount <= 0
                       ? ConsoleColor.Green
                       : ConsoleColor.Red;
                        }
                        _console.Write(diff.Amount.ToString("C").PadLeft(cellColWidth) + "|");
                        _console.ForegroundColor = prevColor; // Reset color after writing

                        // Average Difference per period
                        var avgDiff = periods.Count > 0
                            ? diff.Amount / periods.Count
                            : 0m;

                        // Color Avg Diff using same rules
                        if (isIncome || isSavings)
                        {
                            _console.ForegroundColor = avgDiff >= 0
                                ? ConsoleColor.Green
                                : ConsoleColor.Red;
                        }
                        else
                        {
                            _console.ForegroundColor = avgDiff <= 0
                                ? ConsoleColor.Green
                                : ConsoleColor.Red;
                        }
                        _console.Write(avgDiff.ToString("C").PadLeft(cellColWidth) + "|");
                        _console.ForegroundColor = prevColor;

                        _console.WriteLine();
                        _console.WriteLine(sep);
                    }

                    // Total Savings display below the summary table
                    var totalSavings = periods
                        .Select(d => matrix.ActualByCategoryAndDate
                            .TryGetValue(("Savings", d), out var s) ? s : new Money(0m, defaultCurrency))
                        .Aggregate(new Money(0m, defaultCurrency), (sum, m) => sum + m);
                    _console.WriteLine();
                    _console.WriteLine($"Total Savings: {totalSavings.Amount:C}");
                }
                // Paging controls for all pages
                if (totalPages == 1) break;
                _console.WriteLine("Press [<] Prev, [>] Next, [Q] Quit");
                while (true)
                {
                    var key = _console.ReadKey(true).Key;
                    if (key == ConsoleKey.RightArrow && currentPage < totalPages - 1)
                    {
                        currentPage++;
                        break;
                    }
                    if (key == ConsoleKey.LeftArrow && currentPage > 0)
                    {
                        currentPage--;
                        break;
                    }
                    if (key == ConsoleKey.Q)
                    {
                        _console.Clear();
                        return;
                    }
                }
            }
            _console.Clear();
        }
    }
}
