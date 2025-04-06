using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public static class SavingGoalsReportingHelpers
    {
        public static async Task ViewSavingGoalsReport(IReportingService reportingService)
        {
            try {
                Console.Clear();
                Console.WriteLine("=== Saving Goals Report ===");

                var goals = await reportingService.GenerateSavingGoalReportAsync();
                if (goals.Any())
                {
                    foreach (var goal in goals)
                    {
                        Console.WriteLine($"Goal: {goal.GoalName}");
                        Console.WriteLine($"Target Amount: {goal.TargetAmount:C}");
                        Console.WriteLine($"Current Saved: {goal.CurrentAmount:C}");
                        // Calculate progress percentage
                        decimal progress = goal.TargetAmount > 0 ? (goal.CurrentAmount / goal.TargetAmount * 100) : 0;
                        Console.WriteLine($"Progress: {progress:F2}%");
                        Console.WriteLine(new string('-', 40));
                    }
                }
                else
                {
                    Console.WriteLine("No saving goals found.");
                }

                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey();
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
