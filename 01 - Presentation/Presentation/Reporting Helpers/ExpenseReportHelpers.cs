using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public static class ExpenseReportHelpers
    {
        public static async Task ViewExpenseReport(IReportingService reportingService, InputProcessor inputProcessor)
        {
            Console.Clear();
            Console.WriteLine("=== Detailed Expense Report ===");

            DateTime startDate = inputProcessor.GetValidDate("Enter start date (yyyy-mm-dd): ");
            DateTime endDate = inputProcessor.GetValidDate("Enter end date (yyyy-mm-dd): ");

            var report = await reportingService.GenerateExpenseReportAsync(startDate, endDate);

            Console.WriteLine($"Expense Report ({startDate:d} - {endDate:d})");
            Console.WriteLine($"Total Expenses: {report.TotalExpenses:C}");
            Console.WriteLine("Category Breakdown:");
            foreach (var category in report.CategoryTotals.Keys)
            {
                decimal total = report.CategoryTotals[category];
                decimal percentage = report.CategoryPercentages[category];
                Console.WriteLine($"{category}: {total:C} ({percentage:F2}%)");
            }

            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }


    }
}
