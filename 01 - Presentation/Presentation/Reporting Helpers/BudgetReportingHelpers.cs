// File: Presentation/ReportingHelpers/BudgetReportingHelpers.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Presentation.UIHelpers;
using BudgetTracker.Presentation.PresentationHelpers;

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
            _reportingService = reportingService ?? throw new ArgumentNullException(nameof(reportingService));
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            _budgetService = budgetService ?? throw new ArgumentNullException(nameof(budgetService));
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
        }

        public async Task ViewBudgetRuleReportAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Budget Rule Report ===");
            Guid budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            // 1. Get inputs
            var rule = _input.GetInput("Enter budget rule (e.g., 50/20/30): ");
            var start = _input.GetValidDate("Enter start date (yyyy-MM-dd): ");
            var end = _input.GetValidDate("Enter end date (yyyy-MM-dd): ", allowFuture: true);

            // 2. Generate report
            var report = await _reportingService
                .GenerateBudgetRuleReportAsync(rule, budgetId, start, end);

            // 3. Fetch the budgets native currency code 
            var nativeCurrency = report.InitialCashBalance.Currency;


            // 4. Display Balances
            _console.WriteLine();
            _console.WriteLine("Balances at Budget Start / Now:");
            _console.WriteLine(
                $"  Cash: {await report.InitialCashBalance.ToDisplayAsync(_currencyService)} → " +
                $"{await report.CurrentCashBalance.ToDisplayAsync(_currencyService)}");
            _console.WriteLine(
                $"  Bank: {await report.InitialBankBalance.ToDisplayAsync(_currencyService)} → " +
                $"{await report.CurrentBankBalance.ToDisplayAsync(_currencyService)}");
            _console.WriteLine();

            // 5. Display Rule breakdown
            _console.WriteLine($"Rule: {report.Rule}");
            _console.WriteLine(
                $"  Necessities - Planned: {await report.NecessitiesPlanned.ToDisplayAsync(_currencyService)}, " +
                $"Actual: {await report.NecessitiesActual.ToDisplayAsync(_currencyService)}, " +
                $"Variance: {report.NecessitiesPercentageVariance:F2}%");

            // 6. Display Bulk savings
            var goals = (await _reportingService.GenerateSavingGoalReportAsync(budgetId)).ToList();
            var bulk = goals.FirstOrDefault(g => g.Id == Guid.Empty);

            Money bulkAmt = bulk?.CurrentAmount
                ?? new Money(0m, nativeCurrency);

            Money totalSavingsActual = report.SavingsActual + bulkAmt;

            _console.WriteLine(
                $"  Savings      - Planned: {await report.SavingsPlanned.ToDisplayAsync(_currencyService)}, " +
                $"Actual: {await totalSavingsActual.ToDisplayAsync(_currencyService)} " +
                $"(Goals: {await report.SavingsActual.ToDisplayAsync(_currencyService)} + " +
                $"{await bulkAmt.ToDisplayAsync(_currencyService)}), " +
                $"Variance: {report.SavingsPercentageVariance:F2}%");

            _console.WriteLine(
                $"  Discretion.  - Planned: {await report.DiscretionaryPlanned.ToDisplayAsync(_currencyService)}, " +
                $"Actual: {await report.DiscretionaryActual.ToDisplayAsync(_currencyService)}, " +
                $"Variance: {report.DiscretionaryPercentageVariance:F2}%");

            _console.ReadKey();
        }

        public async Task ViewBudgetMatrixReportAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Budget Matrix Report ===");
            Guid budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty)
            {
                _console.WriteLine("No budget selected. Press any key to return...");
                _console.ReadKey();
                return;
            }

            var matrix = await _reportingService.GenerateBudgetMatrixReportAsync(budgetId);
            var periods = matrix.ReportingPeriods;
            var categories = matrix.Categories;

            const int PageSize = 7;
            var pages = periods
                .Select((date, idx) => new { date, idx })
                .GroupBy(x => x.idx / PageSize)
                .Select(g => g.Select(x => x.date).ToList())
                .ToList();

            const int categoryColWidth = 15, cellColWidth = 15;
            var prevColor = _console.ForegroundColor;
            int currentPage = 0, totalPages = pages.Count + 1;

            while (true)
            {
                _console.Clear();

                if (currentPage < pages.Count)
                {
                    var pageDates = pages[currentPage];

                    _console.WriteLine(
                        $"=== Budget Matrix: {matrix.StartDate:MM/dd/yyyy} – {matrix.EndDate:MM/dd/yyyy} " +
                        $"(Page {currentPage + 1}/{totalPages}) ===");
                    _console.WriteLine("(Each cell: Planned / Actual)");

                    // Separator
                    string sep = "+" + new string('-', categoryColWidth) + "+";
                    sep += string.Concat(Enumerable.Repeat(new string('-', cellColWidth) + "+", pageDates.Count));
                    _console.WriteLine(sep);

                    // Header row
                    _console.Write("| Category".PadRight(categoryColWidth) + "|");
                    foreach (var d in pageDates)
                        _console.Write(d.ToString("MM/yyyy").PadLeft(cellColWidth) + "|");
                    _console.WriteLine();
                    _console.WriteLine(sep.Replace('-', '='));

                    // Income row
                    _console.Write("| Income".PadRight(categoryColWidth) + "|");
                    foreach (var d in pageDates)
                    {
                        matrix.PlannedByCategoryAndDate.TryGetValue(("Income", d), out var pln);
                        matrix.ActualByCategoryAndDate.TryGetValue(("Income", d), out var act);
                        pln ??= new Money(0m, _currencyService.CurrentCurrency);
                        act ??= new Money(0m, _currencyService.CurrentCurrency);

                        string text = $"{await pln.ToDisplayAsync(_currencyService)}/" +
                                      $"{await act.ToDisplayAsync(_currencyService)}";
                        _console.Write(text.PadLeft(cellColWidth) + "|");
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
                            string text = $"{await pln.ToDisplayAsync(_currencyService)}/" +
                                          $"{await act.ToDisplayAsync(_currencyService)}";
                            _console.Write(text.PadLeft(cellColWidth) + "|");
                        }
                        _console.WriteLine();
                        _console.WriteLine(sep);
                    }

                    // Period Totals row
                    _console.Write("| Period Totals".PadRight(categoryColWidth) + "|");
                    foreach (var d in pageDates)
                    {
                        matrix.PlannedByCategoryAndDate.TryGetValue(("Income", d), out var iPln);
                        matrix.ActualByCategoryAndDate.TryGetValue(("Income", d), out var iAct);
                        iPln ??= new Money(0m, _currencyService.CurrentCurrency);
                        iAct ??= new Money(0m, _currencyService.CurrentCurrency);

                        var sumExpPln = categories
                            .Select(c => matrix.PlannedByCategoryAndDate[(c, d)])
                            .Aggregate(new Money(0m, _currencyService.CurrentCurrency), (s, m) => s + m);
                        var sumExpAct = categories
                            .Select(c => matrix.ActualByCategoryAndDate[(c, d)])
                            .Aggregate(new Money(0m, _currencyService.CurrentCurrency), (s, m) => s + m);

                        var totPln = iPln - sumExpPln;
                        var totAct = iAct - sumExpAct;

                        string txtPln = await totPln.ToDisplayAsync(_currencyService);
                        string txtAct = await totAct.ToDisplayAsync(_currencyService);

                        // spacing
                        int pad = cellColWidth - (txtPln.Length + 1 + txtAct.Length);
                        _console.Write(new string(' ', Math.Max(pad, 0)));

                        _console.ForegroundColor = totPln.Amount >= 0 ? ConsoleColor.Green : ConsoleColor.Red;
                        _console.Write(txtPln);
                        _console.ForegroundColor = prevColor;
                        _console.Write("/");

                        _console.ForegroundColor = totAct.Amount >= 0 ? ConsoleColor.Green : ConsoleColor.Red;
                        _console.Write(txtAct);
                        _console.ForegroundColor = prevColor;
                        _console.Write("|");
                    }
                    _console.WriteLine();
                    _console.WriteLine(sep);
                }
                else
                {
                    // Summary page
                    _console.WriteLine(
                        $"=== Summary: {matrix.StartDate:MM/dd/yyyy} – {matrix.EndDate:MM/dd/yyyy} " +
                        $"(Page {currentPage + 1}/{totalPages}) ===");
                    _console.WriteLine("(Planned / Actual → Diff / Avg)");

                    string sep2 = "+" + new string('-', categoryColWidth) + "+" +
                                  new string('-', cellColWidth * 4 + 3) + "+";
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

                        var totPln = periods
                            .Select(d => matrix.PlannedByCategoryAndDate.TryGetValue((cat, d), out var p) ? p : new Money(0m, _currencyService.CurrentCurrency))
                            .Aggregate(new Money(0m, _currencyService.CurrentCurrency), (s, m) => s + m);
                        var totAct = periods
                            .Select(d => matrix.ActualByCategoryAndDate.TryGetValue((cat, d), out var a) ? a : new Money(0m, _currencyService.CurrentCurrency))
                            .Aggregate(new Money(0m, _currencyService.CurrentCurrency), (s, m) => s + m);

                        var diff = totAct - totPln;
                        decimal avg = periods.Count > 0 ? diff.Amount / periods.Count : 0m;
                        var avgMoney = new Money(avg, _currencyService.CurrentCurrency);

                        _console.Write((await totPln.ToDisplayAsync(_currencyService)).PadLeft(cellColWidth) + "|");
                        _console.Write((await totAct.ToDisplayAsync(_currencyService)).PadLeft(cellColWidth) + "|");

                        bool isInc = cat.Equals("Income", StringComparison.OrdinalIgnoreCase);
                        bool isSav = cat.Equals("Savings", StringComparison.OrdinalIgnoreCase);

                        _console.ForegroundColor = (isInc || isSav ? diff.Amount >= 0 : diff.Amount <= 0)
                            ? ConsoleColor.Green : ConsoleColor.Red;
                        _console.Write((await diff.ToDisplayAsync(_currencyService)).PadLeft(cellColWidth) + "|");
                        _console.ForegroundColor = prevColor;

                        _console.ForegroundColor = (isInc || isSav ? avgMoney.Amount >= 0 : avgMoney.Amount <= 0)
                            ? ConsoleColor.Green : ConsoleColor.Red;
                        _console.Write((await avgMoney.ToDisplayAsync(_currencyService)).PadLeft(cellColWidth) + "|");
                        _console.ForegroundColor = prevColor;

                        _console.WriteLine();
                        _console.WriteLine(sep2);
                    }

                    var totalSav = periods
                        .Select(d => matrix.ActualByCategoryAndDate.TryGetValue(("Savings", d), out var s) ? s : new Money(0m, _currencyService.CurrentCurrency))
                        .Aggregate(new Money(0m, _currencyService.CurrentCurrency), (sum, m) => sum + m);

                    _console.WriteLine();
                    _console.WriteLine($"Total Savings: {await totalSav.ToDisplayAsync(_currencyService)}");
                }

                // paging
                if (totalPages == 1) break;
                _console.WriteLine("Press [<] Prev, [>] Next, [Q] Quit");
                var key = _console.ReadKey(true).Key;
                if (key == ConsoleKey.RightArrow && currentPage < totalPages - 1) currentPage++;
                else if (key == ConsoleKey.LeftArrow && currentPage > 0) currentPage--;
                else if (key == ConsoleKey.Q) break;
            }

            _console.WriteLine("\nPress any key to return to Reports menu...");
            _console.ReadKey();
            _console.Clear();
        }
    }
}
