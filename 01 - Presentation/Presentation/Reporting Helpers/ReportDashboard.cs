using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public static class ReportDashboard
    {
        public static async Task ViewDashboard(IReportingService reportingService)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== Dashboard Summary ===");

                // Using the last 30 days for budget and income reports.
                var budgetReport = await reportingService.GenerateBudgetReportAsync();
                var incomeReport = await reportingService.GenerateIncomeReportAsync(DateTime.Now.AddDays(-30), DateTime.Now);
                var savingGoals = await reportingService.GenerateSavingGoalReportAsync();

                Console.WriteLine("Budget Summary:");
                Console.WriteLine($"Planned Budget: {budgetReport.BudgetedExpenses:C}");
                Console.WriteLine($"Actual Expenses: {budgetReport.ActualExpenses:C}");
                Console.WriteLine($"Difference: {budgetReport.Difference:C}");
                Console.WriteLine();

                Console.WriteLine("Income Summary:");
                Console.WriteLine($"Total Income (Last 30 days): {incomeReport.TotalIncome:C}");
                Console.WriteLine();

                Console.WriteLine("Saving Goals Summary:");
                if (savingGoals.Any())
                {
                    foreach (var goal in savingGoals)
                    {
                        decimal progress = goal.TargetAmount > 0 ? (goal.CurrentAmount / goal.TargetAmount * 100) : 0;
                        Console.WriteLine($"{goal.GoalName}: {goal.CurrentAmount:C} / {goal.TargetAmount:C} ({progress:F2}%)");
                    }
                }
                else
                {
                    Console.WriteLine("No saving goals defined.");
                }
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
    
