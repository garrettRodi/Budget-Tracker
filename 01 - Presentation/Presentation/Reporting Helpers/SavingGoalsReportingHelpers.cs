using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public static class SavingGoalsReportingHelpers
    {
        public static async Task ViewSavingGoalsReport(IReportingService reportingService,IBudgetService budgetService)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== Saving Goals Report ===");

                var selector = new BudgetSelector(budgetService);
                Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();

                if (activeBudgetId == Guid.Empty)
                {
                    Console.WriteLine("No active budget found. Please create a budget first.");
                    return;
                }

                var goals = await reportingService.GenerateSavingGoalReportAsync(activeBudgetId);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while generating the budget report.");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
