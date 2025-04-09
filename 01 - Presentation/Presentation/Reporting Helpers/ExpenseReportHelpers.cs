using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Services;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public static class ExpenseReportHelpers
    {
        public static async Task ViewExpenseReport(IReportingService reportingService, InputProcessor inputProcessor, IBudgetService budgetService)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== Detailed Expense Report ===");

                var selector = new BudgetSelector(budgetService);
                Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();

                if (activeBudgetId == Guid.Empty)
                {
                    Console.WriteLine("No active budget found. Please create a budget first.");
                    return;
                }

                DateTime startDate = inputProcessor.GetValidDate("Enter start date (yyyy-mm-dd): ");
                DateTime endDate = inputProcessor.GetValidDate("Enter end date (yyyy-mm-dd): ");

                var report = await reportingService.GenerateExpenseReportAsync(activeBudgetId, startDate, endDate);

                Console.WriteLine($"Expense Report ({startDate:d} - {endDate:d})");
                Console.WriteLine($"Total Expenses: {report.TotalExpenses:C}");
                Console.WriteLine("Category Breakdown:");
                foreach (var category in report.CategoryTotals.Keys)
                {
                    decimal total = report.CategoryTotals[category];
                    decimal percentage = report.CategoryPercentages[category];
                    Console.WriteLine($"{category}: {total:C} ({percentage:F2}%)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while generating the budget report.");
                Console.WriteLine(ex.Message);
                // If you have a logger available, you might log the exception here.
            }
        }
    }
}
