using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public static class DrillDown
    {
        public static async Task DrillDownReport(IReportingService reportingService, InputProcessor inputProcessor)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== Drill-Down Expense Report ===");

                string category = inputProcessor.GetInput("Enter the expense category to drill down (e.g., Food): ");
                DateTime startDate = inputProcessor.GetValidDate("Enter start date (yyyy-mm-dd): ");
                DateTime endDate = inputProcessor.GetValidDate("Enter end date (yyyy-mm-dd): ");

                var report = await reportingService.GenerateExpenseReportAsync(startDate, endDate);

                if (report.CategoryTotals.TryGetValue(category, out decimal categoryTotal))
                {
                    decimal percentage = report.CategoryPercentages[category];
                    Console.WriteLine($"Category: {category}");
                    Console.WriteLine($"Total Expenses: {categoryTotal:C}");
                    Console.WriteLine($"Percentage of total expenses: {percentage:F2}%");
                }
                else
                {
                    Console.WriteLine($"No expenses found for category '{category}'.");
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
