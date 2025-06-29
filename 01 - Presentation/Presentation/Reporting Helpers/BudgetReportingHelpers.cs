// File: Presentation/ReportingHelpers/BudgetReportingHelpers.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Services;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Presentation.PresentationHelpers;
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
            var end = _input.GetValidDate("Enter end date (yyyy-MM-dd): ", allowFuture: true);

            var report = await _reportingService.GenerateBudgetRuleReportAsync(rule, budgetId, start, end);

            // ——— INTEGRATE BALANCES ———
            _console.WriteLine();
            _console.WriteLine("Balances at Budget Start / Now:");
            _console.WriteLine($"  Cash: {report.InitialCashBalance.ToDisplay(_currencyService)} → {report.CurrentCashBalance.ToDisplay(_currencyService)}");
            _console.WriteLine($"  Bank: {report.InitialBankBalance.ToDisplay(_currencyService)} → {report.CurrentBankBalance.ToDisplay(_currencyService)}");
            _console.WriteLine();
            // ————————————————————————

            _console.WriteLine($"Rule: {report.Rule}");
            _console.WriteLine($"  Necessities - Planned: {report.NecessitiesPlanned.ToDisplay(_currencyService)}, Actual: {report.NecessitiesActual.ToDisplay(_currencyService)}, Variance: {report.NecessitiesPercentageVariance:F2}%");

            // Add uncategorized/ bulk savings to the Savings line
            var savingGoalsReport = (await _reportingService.GenerateSavingGoalReportAsync(budgetId)).ToList();
            var bulk = savingGoalsReport.FirstOrDefault(g => g.Id == Guid.Empty);
            Money bulkAmount = bulk?.CurrentAmount ?? new Money(0m, _currencyService.CurrentCurrency); ;

            Money totalSavingsActual = report.SavingsActual + bulkAmount;
            _console.WriteLine($"  Savings      - Planned: {report.SavingsPlanned.ToDisplay(_currencyService)}, Actual: {totalSavingsActual:C} (Goals: {report.SavingsActual.ToDisplay(_currencyService)} + Bulk: {bulkAmount:C}), Variance: {report.SavingsPercentageVariance:F2}%");
            _console.WriteLine($"  Discretion.  - Planned: {report.DiscretionaryPlanned.ToDisplay(_currencyService)}, Actual: {report.DiscretionaryActual.ToDisplay(_currencyService)}, Variance: {report.DiscretionaryPercentageVariance:F2}%");
            _console.ReadKey();
        }
        public async Task ViewBudgetMatrixReportAsync()
        {
            // 1) Select the budget
            _console.Clear();
            _console.WriteLine("=== Budget Matrix Report ===");
            Guid budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty)
            {
                _console.WriteLine("No budget selected. Press any key to return...");
                _console.ReadKey();
                return;
            }

            // 2) Generate the matrix DTO
            var matrix = await _reportingService.GenerateBudgetMatrixReportAsync(budgetId);
            var periods = matrix.ReportingPeriods;
            var categories = matrix.Categories;

            // 3) Split into pages
            const int PageSize = 7;
            var pages = periods
                .Select((date, idx) => new { date, idx })
                .GroupBy(x => x.idx / PageSize)
                .Select(g => g.Select(x => x.date).ToList())
                .ToList();

            // 4) Layout settings
            const int categoryColWidth = 15;
            const int cellColWidth = 15;
            var prevColor = _console.ForegroundColor;
            bool done = false;
            int currentPage = 0;
            int totalPages = pages.Count + 1; // +1 for summary page

            // 5) Loop until user quits with Q on paging
            while (!done)
            {
                _console.Clear();

                // 5a) Data pages
                if (currentPage < pages.Count)
                {
                    var pageDates = pages[currentPage];

                    // Header
                    _console.WriteLine(
                        $"=== Budget Matrix: {matrix.StartDate:MM/dd/yyyy} – {matrix.EndDate:MM/dd/yyyy} " +
                        $"(Page {currentPage + 1}/{totalPages}) ===");
                    _console.WriteLine("(Each cell: Planned / Actual)");

                    // Separator line
                    string sep = "+" + new string('-', categoryColWidth) + "+";
                    sep += string.Concat(Enumerable.Repeat(new string('-', cellColWidth) + "+", pageDates.Count));
                    _console.WriteLine(sep);

                    // Column titles
                    _console.Write("| Category".PadRight(categoryColWidth) + "|");
                    foreach (var d in pageDates)
                        _console.Write(d.ToString("MM/yyyy").PadLeft(cellColWidth) + "|");
                    _console.WriteLine();
                    _console.WriteLine(sep.Replace('-', '='));

                    // Income row
                    _console.Write("| Income".PadRight(categoryColWidth) + "|");
                    foreach (var d in pageDates)
                    {
                        matrix.PlannedByCategoryAndDate.TryGetValue(("Income", d), out var incomePln);
                        matrix.ActualByCategoryAndDate.TryGetValue(("Income", d), out var incomeAct);
                        incomePln ??= new Money(0m, _currencyService.CurrentCurrency);
                        incomeAct ??= new Money(0m, _currencyService.CurrentCurrency);

                        string plnText = incomePln.ToDisplay(_currencyService);
                        string actText = incomeAct.ToDisplay(_currencyService);
                        string incomeCell = $"{plnText}/{actText}".PadLeft(cellColWidth);
                        _console.Write(incomeCell + "|");
                    }
                    _console.WriteLine();
                    _console.WriteLine(sep);

                    // Category rows
                    foreach (var cat in categories)
                    {
                        _console.Write("| " + cat.PadRight(categoryColWidth - 2) + " |");
                        foreach (var d in pageDates)
                        {
                            var pln = matrix.PlannedByCategoryAndDate[(cat, d)];
                            var act = matrix.ActualByCategoryAndDate[(cat, d)];

                            string plnText = pln.ToDisplay(_currencyService);
                            string actText = act.ToDisplay(_currencyService);
                            string cell = $"{plnText}/{actText}".PadLeft(cellColWidth);
                            _console.Write(cell + "|");
                        }
                        _console.WriteLine();
                        _console.WriteLine(sep);
                    }

                    // Period Totals row
                    _console.Write("| Period Totals".PadRight(categoryColWidth) + "|");
                    foreach (var d in pageDates)
                    {
                        matrix.PlannedByCategoryAndDate.TryGetValue(("Income", d), out var incomePln);
                        matrix.ActualByCategoryAndDate.TryGetValue(("Income", d), out var incomeAct);
                        incomePln ??= new Money(0m, _currencyService.CurrentCurrency);
                        incomeAct ??= new Money(0m, _currencyService.CurrentCurrency);

                        var expPln = categories
                            .Select(c => matrix.PlannedByCategoryAndDate[(c, d)])
                            .Aggregate(new Money(0m, _currencyService.CurrentCurrency), (sum, m) => sum + m);
                        var expAct = categories
                            .Select(c => matrix.ActualByCategoryAndDate[(c, d)])
                            .Aggregate(new Money(0m, _currencyService.CurrentCurrency), (sum, m) => sum + m);

                        var totPln = incomePln - expPln;
                        var totAct = incomeAct - expAct;

                        string totPlnText = totPln.ToDisplay(_currencyService);
                        string totActText = totAct.ToDisplay(_currencyService);

                        int padding = cellColWidth - (totPlnText.Length + 1 + totActText.Length);
                        _console.Write(new string(' ', Math.Max(padding, 0)));

                        _console.ForegroundColor = totPln.Amount >= 0 ? ConsoleColor.Green : ConsoleColor.Red;
                        _console.Write(totPlnText);
                        _console.ForegroundColor = prevColor;
                        _console.Write("/");

                        _console.ForegroundColor = totAct.Amount >= 0 ? ConsoleColor.Green : ConsoleColor.Red;
                        _console.Write(totActText);
                        _console.ForegroundColor = prevColor;
                        _console.Write("|");
                    }
                    _console.WriteLine();
                    _console.WriteLine(sep);
                }
                else
                {
                    // 5b) Summary page
                    _console.WriteLine(
                        $"=== Summary: {matrix.StartDate:MM/dd/yyyy} – {matrix.EndDate:MM/dd/yyyy} " +
                        $"(Page {currentPage + 1}/{totalPages}) ===");
                    _console.WriteLine("(Planned / Actual → Diff / Avg)");

                    string sep2 = "+" + new string('-', categoryColWidth) + "+"
                                + new string('-', cellColWidth * 4 + 3) + "+";
                    _console.WriteLine(sep2.Replace('-', '='));

                    _console.Write("| Category".PadRight(categoryColWidth) + "|");
                    _console.Write("Total Planned".PadLeft(cellColWidth) + "|");
                    _console.Write("Total Actual".PadLeft(cellColWidth) + "|");
                    _console.Write("Total Diff".PadLeft(cellColWidth) + "|");
                    _console.Write("Avg Diff".PadLeft(cellColWidth) + "|\n");
                    _console.WriteLine(sep2);

                    var allCats = new[] { "Income" }.Concat(categories);
                    foreach (var cat in allCats)
                    {
                        _console.Write("| " + cat.PadRight(categoryColWidth - 2) + " |");

                        var totalPln = periods
                            .Select(d => matrix.PlannedByCategoryAndDate.TryGetValue((cat, d), out var p) ? p : new Money(0m, _currencyService.CurrentCurrency))
                            .Aggregate(new Money(0m, _currencyService.CurrentCurrency), (s, m) => s + m);
                        var totalAct = periods
                            .Select(d => matrix.ActualByCategoryAndDate.TryGetValue((cat, d), out var a) ? a : new Money(0m, _currencyService.CurrentCurrency))
                            .Aggregate(new Money(0m, _currencyService.CurrentCurrency), (s, m) => s + m);

                        var diff = totalAct - totalPln;
                        decimal avgVal = periods.Count > 0 ? diff.Amount / periods.Count : 0m;
                        var avgMoney = new Money(avgVal, _currencyService.CurrentCurrency);

                        _console.Write(totalPln.ToDisplay(_currencyService).PadLeft(cellColWidth) + "|");
                        _console.Write(totalAct.ToDisplay(_currencyService).PadLeft(cellColWidth) + "|");

                        bool isInc = cat.Equals("Income", StringComparison.OrdinalIgnoreCase);
                        bool isSav = cat.Equals("Savings", StringComparison.OrdinalIgnoreCase);

                        _console.ForegroundColor = (isInc || isSav ? diff.Amount >= 0 : diff.Amount <= 0)
                            ? ConsoleColor.Green : ConsoleColor.Red;
                        _console.Write(diff.ToDisplay(_currencyService).PadLeft(cellColWidth) + "|");
                        _console.ForegroundColor = prevColor;

                        _console.ForegroundColor = (isInc || isSav ? avgMoney.Amount >= 0 : avgMoney.Amount <= 0)
                            ? ConsoleColor.Green : ConsoleColor.Red;
                        _console.Write(avgMoney.ToDisplay(_currencyService).PadLeft(cellColWidth) + "|");
                        _console.ForegroundColor = prevColor;

                        _console.WriteLine();
                        _console.WriteLine(sep2);
                    }

                    var totalSavings = periods
                        .Select(d => matrix.ActualByCategoryAndDate.TryGetValue(("Savings", d), out var s) ? s : new Money(0m, _currencyService.CurrentCurrency))
                        .Aggregate(new Money(0m, _currencyService.CurrentCurrency), (sum, m) => sum + m);

                    _console.WriteLine();
                    _console.WriteLine($"Total Savings: {totalSavings.ToDisplay(_currencyService)}");
                }

                // 6) Paging controls
                if (totalPages == 1) break;
                _console.WriteLine("Press [<] Prev, [>] Next, [Q] Quit");
                var key = _console.ReadKey(true).Key;
                if (key == ConsoleKey.RightArrow && currentPage < totalPages - 1)
                    currentPage++;
                else if (key == ConsoleKey.LeftArrow && currentPage > 0)
                    currentPage--;
                else if (key == ConsoleKey.Q)
                    break;
            }

            // 7) Final pause before returning
            _console.WriteLine("\nPress any key to return to Reports menu...");
            _console.ReadKey();
            _console.Clear();
        }

    }
}
