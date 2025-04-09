using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Services;
using BudgetTracker.Presentation.UIHelpers;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public static class BudgetReportingHelpers
    {
        public static async Task ViewBudgetReport(IReportingService reportingService,InputProcessor inputProcessor, IBudgetService budgetService )
        {

            try
            {
                Console.Clear();
                Console.WriteLine("=== Comprehensive Budget Report ===");

                var selector = new BudgetSelector(budgetService);
                Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();
                if (activeBudgetId == Guid.Empty)
                {
                    Console.WriteLine("No active budget found. Please create a budget first.");
                    return;
                }

                DateTime startDate = inputProcessor.GetValidDate("Enter start date (yyyy-mm-dd): ");
                DateTime endDate = inputProcessor.GetValidDate("Enter end date (yyyy-mm-dd): ");

                var report = await reportingService.GenerateBudgetReportAsync(activeBudgetId);

                Console.WriteLine($"Budget Report for Budget {activeBudgetId}");
                Console.WriteLine($"Planned Budget: {report.BudgetedExpenses:C}");
                Console.WriteLine($"Actual Expenses: {report.ActualExpenses:C}");
                Console.WriteLine($"Difference: {report.Difference:C}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while generating the budget report.");
                Console.WriteLine(ex.Message);
            }
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
