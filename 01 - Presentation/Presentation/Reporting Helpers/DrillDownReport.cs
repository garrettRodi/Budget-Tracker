using System;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public static class DrillDown
    {
        public static async Task DrillDownReport(IReportingService reportingService, InputProcessor inputProcessor, IBudgetService budgetService)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== Drill-Down Expense Report ===");

                // Get the active budget container.
                var selector = new BudgetSelector(budgetService);
                Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();
                if (activeBudgetId == Guid.Empty)
                {
                    Console.WriteLine("No active budget found. Please create a budget first.");
                    return;
                }

                string category = inputProcessor.GetInput("Enter the expense category to drill down (e.g., Food): ");
                DateTime startDate = inputProcessor.GetValidDate("Enter start date (yyyy-mm-dd): ");
                DateTime endDate = inputProcessor.GetValidDate("Enter end date (yyyy-mm-dd): ");

                // Call the new ReportingService method.
                var report = await reportingService.GetFilteredExpensesAsync(activeBudgetId, category, startDate, endDate);
                if (report.CategoryTotals.TryGetValue(category, out decimal categoryTotal))
                {
                    decimal percentage = report.CategoryPercentages[category];
                    Console.WriteLine($"Drill-Down Report for Category: {category}");
                    Console.WriteLine($"Total Expenses: {categoryTotal:C}");
                    Console.WriteLine($"Percentage of total expenses: {percentage:F2}%");
                }
                else
                {
                    Console.WriteLine($"No expenses found for category '{category}' in the given period.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while generating the drill-down report: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey();
            }
        }
    }
}
