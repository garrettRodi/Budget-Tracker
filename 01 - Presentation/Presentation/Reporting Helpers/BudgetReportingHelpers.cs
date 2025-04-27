using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Services;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Presentation.UIHelpers;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public static class BudgetReportingHelpers
    {
        public static async Task ViewBudgetMatrixReportAsync(
    IReportingService reportingService,
    InputProcessor inputProcessor,
    IBudgetService budgetService)
        {
            // 1) Get the active budget and its matrix
            var selector = new BudgetSelector(budgetService);
            var budgetId = await selector.GetActiveBudgetContainerIdAsync();
            var matrix = await reportingService.GenerateBudgetMatrixReportAsync(budgetId);

            // 2) Build the list of dates in the period
            var dates = Enumerable
                .Range(0, (matrix.EndDate - matrix.StartDate).Days + 1)
                .Select(offset => matrix.StartDate.AddDays(offset))
                .ToList();

            // 3) Page size (days per page)
            const int PageSize = 7;
            var pages = dates
                .Select((date, idx) => new { date, idx })
                .GroupBy(x => x.idx / PageSize)
                .Select(g => g.Select(x => x.date).ToList())
                .ToList();

            // 4) For each page, render a little table
            for (int p = 0; p < pages.Count; p++)
            {
                var pageDates = pages[p];

                Console.Clear();
                Console.WriteLine(
                    $"=== Budget Matrix: “{matrix.StartDate:MM/dd/yyyy} – {matrix.EndDate:MM/dd/yyyy}” " +
                    $"(Page {p + 1}/{pages.Count}) ===\n");

                // --- HEADER LINE ---
                // corner +---+ and one cell per date plus three total columns
                string sep = "+" + new string('-', 15) + "+";
                foreach (var _ in pageDates) sep += new string('-', 13) + "+";
                sep += new string('-', 11) + "+" + new string('-', 11) + "+" + new string('-', 9) + "+";
                Console.WriteLine(sep);

                // title row
                Console.Write("| Category".PadRight(15) + "|");
                foreach (var d in pageDates)
                    Console.Write(d.ToString("MM/dd").PadLeft(12) + "|");
                Console.Write(" TotPln".PadLeft(11) + "|" + " TotAct".PadLeft(11) + "|" + " Diff".PadLeft(9) + "|\n");

                // separator under header
                Console.WriteLine(sep.Replace('-', '='));

                // --- DATA ROWS ---
                foreach (var cat in matrix.Categories)
                {
                    Console.Write("| " + cat.PadRight(13) + "|");

                    decimal rowPln = 0, rowAct = 0;
                    foreach (var d in pageDates)
                    {
                        matrix.PlannedByCategoryAndDate.TryGetValue((cat, d), out var pln);
                        matrix.ActualByCategoryAndDate.TryGetValue((cat, d), out var act);
                        rowPln += pln;
                        rowAct += act;
                        Console.Write($"{pln,6:C}/{act,-6:C}|");
                    }

                    var diff = rowPln - rowAct;
                    Console.Write($"{rowPln,9:C}|{rowAct,11:C}|{diff,9:C}|\n");

                    // row separator
                    Console.WriteLine(sep);
                }

                // 5) Pause between pages
                if (pages.Count > 1 && p < pages.Count - 1)
                {
                    Console.Write("Press any key for next page...");
                    Console.ReadKey(true);
                }
            }

            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey(true);
        }


        public static async Task ViewBudgetRuleReport(IReportingService reportingService, InputProcessor inputProcessor, IBudgetService budgetService)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== Budget Rule Report ===");

                var selector = new BudgetSelector(budgetService);
                Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();
                if (activeBudgetId == Guid.Empty)
                {
                    Console.WriteLine("No active budget found. Please create a budget first.");
                    return;
                }

                string rule = inputProcessor.GetInput("Enter budget rule (e.g., 50/20/30): ");

                DateTime startDate = inputProcessor.GetValidDate("Enter start date (yyyy-mm-dd): ");
                DateTime endDate = inputProcessor.GetValidDate("Enter end date (yyyy-mm-dd): ");

                var report = await reportingService.GenerateBudgetRuleReportAsync(rule, activeBudgetId, startDate, endDate);

                Console.WriteLine($"Budget Rule: {report.Rule}");
                Console.WriteLine($"Necessities - Planned: {report.NecessitiesPlanned:C}, Actual: {report.NecessitiesActual:C}, Variance: {report.NecessitiesPercentageVariance:F2}%");
                Console.WriteLine($"Savings - Planned: {report.SavingsPlanned:C}, Actual: {report.SavingsActual:C}, Variance: {report.SavingsPercentageVariance:F2}%");
                Console.WriteLine($"Discretionary - Planned: {report.DiscretionaryPlanned:C}, Actual: {report.DiscretionaryActual:C}, Variance: {report.DiscretionaryPercentageVariance:F2}%");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while generating the budget report.");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
