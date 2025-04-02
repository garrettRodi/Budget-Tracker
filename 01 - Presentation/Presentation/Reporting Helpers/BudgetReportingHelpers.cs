using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public static class BudgetReportingHelpers
    {
        public static async Task ViewBudgetReport(IReportingService reportingService)
        {
            Console.Clear();
            Console.WriteLine("=== Comprehensive Budget Report ===");

            var report = await reportingService.GenerateBudgetReportAsync();
            Console.WriteLine($"Planned Budget: {report.BudgetedExpenses:C}");
            Console.WriteLine($"Actual Expenses: {report.ActualExpenses:C}");
            Console.WriteLine($"Difference: {report.Difference:C}");

            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }

        public static async Task ViewBudgetRuleReport(IReportingService reportingService, InputProcessor inputProcessor)
        {
            Console.Clear();
            Console.WriteLine("=== Budget Rule Report ===");

            string rule = inputProcessor.GetInput("Enter budget rule (e.g., 50/20/30): ");
            DateTime startDate = inputProcessor.GetValidDate("Enter start date (yyyy-mm-dd): ");
            DateTime endDate = inputProcessor.GetValidDate("Enter end date (yyyy-mm-dd): ");

            var report = await reportingService.GenerateBudgetRuleReportAsync(rule, startDate, endDate);

            Console.WriteLine($"Budget Rule: {report.Rule}");
            Console.WriteLine($"Necessities - Planned: {report.NecessitiesPlanned:C}, Actual: {report.NecessitiesActual:C}, Variance: {report.NecessitiesPercentageVariance:F2}%");
            Console.WriteLine($"Savings - Planned: {report.SavingsPlanned:C}, Actual: {report.SavingsActual:C}, Variance: {report.SavingsPercentageVariance:F2}%");
            Console.WriteLine($"Discretionary - Planned: {report.DiscretionaryPlanned:C}, Actual: {report.DiscretionaryActual:C}, Variance: {report.DiscretionaryPercentageVariance:F2}%");

            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }
    }
}
