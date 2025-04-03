using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public static class IncomeReportingHelpers
    {
        public static async Task ViewIncomeReport(IReportingService reportingService, InputProcessor inputProcessor)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== Income Report ===");

                DateTime startDate = inputProcessor.GetValidDate("Enter start date (yyyy-mm-dd): ");
                DateTime endDate = inputProcessor.GetValidDate("Enter end date (yyyy-mm-dd): ");

                var report = await reportingService.GenerateIncomeReportAsync(startDate, endDate);

                Console.WriteLine($"Income Report ({startDate:d} - {endDate:d})");
                Console.WriteLine($"Total Income: {report.TotalIncome:C}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while generating the budget report.");
                Console.WriteLine(ex.Message);
                // If you have a logger available, you might log the exception here.
            }
            finally
            {
                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey();
            }
        }
    }
}
