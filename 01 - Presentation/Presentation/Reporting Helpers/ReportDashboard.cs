// File: Presentation/ReportingHelpers/ReportDashboard.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.ReportingHelpers
{
    public class ReportDashboard
    {
        private readonly IReportingService _reportingService;
        private readonly IExpenseService _expenseService;
        private readonly SelectBudgetContainer _selector;
        private readonly IConsole _console;

        public ReportDashboard(
            IReportingService reportingService,
            IExpenseService expenseService,
            SelectBudgetContainer selector,
            IConsole console)
        {
            _reportingService = reportingService
                ?? throw new ArgumentNullException(nameof(reportingService));
            _expenseService = expenseService
                ?? throw new ArgumentNullException(nameof(expenseService));
            _selector = selector
                ?? throw new ArgumentNullException(nameof(selector));
            _console = console
                ?? throw new ArgumentNullException(nameof(console));
        }

        public async Task ViewDashboardAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Dashboard Summary ===");

            var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
            if (budgetId == Guid.Empty) return;

            // 1. Budget summary
            var budgetReport = await _reportingService.GenerateBudgetReportAsync(budgetId);
            _console.WriteLine($"Planned Budget: {budgetReport.BudgetedExpenses:C}");
            _console.WriteLine($"Actual Expenses: {budgetReport.ActualExpenses:C}");
            _console.WriteLine($"Difference: {budgetReport.Difference:C}\n");

            // 2. Income last 30 days
            var incomeStart = DateTime.Now.AddDays(-30);
            var incomeEnd = DateTime.Now;
            var incomeReport = await _reportingService.GenerateIncomeReportAsync(budgetId, incomeStart, incomeEnd);
            _console.WriteLine("Income (Last 30 Days):");
            _console.WriteLine($"  Total Income: {incomeReport.TotalIncome:C}\n");

            // 3. Saving goals progress + bulk savings (total savings = goals + bulk)
            var savingGoals = (await _reportingService.GenerateSavingGoalReportAsync(budgetId)).ToList();

            // Split out the "bulk/unallocated" (ID == Guid.Empty) and named goals
            var namedGoals = savingGoals.Where(g => g.Id != Guid.Empty).ToList();
            var bulkGoal = savingGoals.FirstOrDefault(g => g.Id == Guid.Empty);

            _console.WriteLine("Saving Goals:");
            if (namedGoals.Any())
            {
                foreach (var g in namedGoals)
                {
                    var progress = g.TargetAmount > 0
                        ? g.CurrentAmount / g.TargetAmount * 100
                        : 0;
                    _console.WriteLine($"  {g.GoalName}: {g.CurrentAmount:C} / {g.TargetAmount:C} ({progress:F2}%)");
                }
            }
            else
            {
                _console.WriteLine("  No saving goals found.");
            }

            // Show "Bulk/Uncategorized Savings" if present
            if (bulkGoal != null && bulkGoal.CurrentAmount > 0)
            {
                _console.WriteLine($"  {bulkGoal.GoalName}: {bulkGoal.CurrentAmount:C} (Bulk/Uncategorized)");
            }

            // Show total savings (all goals + bulk)
            decimal totalSavings = savingGoals.Sum(g => g.CurrentAmount);
            _console.WriteLine($"\nTotal Savings (all goals + bulk): {totalSavings:C}");

            // 4. Most recent savings expense
            var allExpenses = await _expenseService.GetExpensesByBudgetContainerIdAsync(budgetId);
            var recentSaving = allExpenses
                .Where(e => e.Category.Equals("Savings", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(e => e.Date)
                .FirstOrDefault();
            if (recentSaving != null)
            {
                _console.WriteLine("\nMost Recent Saving Expense:");
                _console.WriteLine($"  ID: {recentSaving.Id}");
                _console.WriteLine($"  Name: {recentSaving.Name}");
                _console.WriteLine($"  Amount: {recentSaving.Amount:C}");
                _console.WriteLine($"  Date: {recentSaving.Date:yyyy-MM-dd}");
            }
            else
            {
                _console.WriteLine("\nNo saving expenses found.");
            }
            _console.ReadKey();
        }
    }
}
