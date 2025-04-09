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
    public static class ReportDashboard
    {
        public static async Task ViewDashboard(IReportingService reportingService, InputProcessor inputProcessor, IBudgetService budgetService, IExpenseService expenseService)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== Dashboard Summary ===");

                // Obtain the active budget container ID using BudgetSelector.
                var selector = new BudgetSelector(budgetService);
                Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();
                if (activeBudgetId == Guid.Empty)
                {
                    Console.WriteLine("No active budget found. Please create a budget first.");
                    return;
                }

                // Dashboard: Use the entire container's data.
                // 1. Get the budget report for the active budget.
                var budgetReport = await reportingService.GenerateBudgetReportAsync(activeBudgetId);

                // 2. Get the income report for the last 30 days.
                DateTime incomeStart = DateTime.Now.AddDays(-30);
                DateTime incomeEnd = DateTime.Now;
                var incomeReport = await reportingService.GenerateIncomeReportAsync(activeBudgetId, incomeStart, incomeEnd);

                // 3. Get the saving goals report for the active budget.
                var savingGoalsReport = await reportingService.GenerateSavingGoalReportAsync(activeBudgetId);

                // Display the Dashboard:
                Console.WriteLine("Budget Summary:");
                Console.WriteLine($"Planned Budget: {budgetReport.BudgetedExpenses:C}");
                Console.WriteLine($"Actual Expenses: {budgetReport.ActualExpenses:C}");
                Console.WriteLine($"Difference: {budgetReport.Difference:C}");
                Console.WriteLine();

                Console.WriteLine("Income Summary (Last 30 Days):");
                Console.WriteLine($"Total Income: {incomeReport.TotalIncome:C}");
                Console.WriteLine();

                Console.WriteLine("Saving Goals Summary:");
                if (savingGoalsReport.Any())
                {
                    foreach (var goal in savingGoalsReport)
                    {
                        decimal progress = goal.TargetAmount > 0 ? (goal.CurrentAmount / goal.TargetAmount * 100) : 0;
                        Console.WriteLine($"{goal.GoalName}: {goal.CurrentAmount:C} / {goal.TargetAmount:C} ({progress:F2}%)");
                    }
                }
                else
                {
                    Console.WriteLine("No saving goals found.");
                }

                var savingExpenseReport = await expenseService.GetExpensesByBudgetContainerIdAsync(activeBudgetId);

                var recentSavingsExpense = savingExpenseReport
                    .Where(e => e.Category.ToLower() == "savings")
                .OrderByDescending(e => e.Date)  // Assuming the property is Date in ExpenseDTO
                .FirstOrDefault();

                if (recentSavingsExpense != null)
                {
                    Console.WriteLine("\nMost Recent Saving Expense:");
                    Console.WriteLine($"ID: {recentSavingsExpense.Id}");
                    Console.WriteLine($"Name: {recentSavingsExpense.Name}");
                    Console.WriteLine($"Amount: {recentSavingsExpense.Amount:C}");
                    Console.WriteLine($"Date: {recentSavingsExpense.Date:d}");
                    Console.WriteLine($"Category: {recentSavingsExpense.Category}");
                }
                else
                {
                    Console.WriteLine("No saving expense found for the active budget.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while generating the dashboard report.");
                Console.WriteLine(ex.Message);
                // If you have a logger available, you might log the exception here.
            }
        }
    }
}



