using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.SavingGoalsHelpers
{
    public class SavingGoalsHelpers
    {
        public static async Task CreateSavingGoal(ISavingGoalsService savingGoalsService, InputProcessor inputProcessor, IBudgetService budgetService)
        {
            Console.Clear();
            Console.WriteLine("=== Create Saving Goal ===");

            var selector = new BudgetSelector(budgetService);
            Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();

            if (activeBudgetId == Guid.Empty)
            {
                Console.WriteLine("No active budget found. Please create a budget first.");
                return;
            }

            string goalName = inputProcessor.GetInput("Enter saving goal name: ");
            decimal targetAmount = inputProcessor.GetValidDecimal("Enter target amount: ");
            decimal currentAmount = inputProcessor.GetValidDecimal("Enter current amount: ");
            DateTime? targetDate = inputProcessor.GetValidDate("Enter target date (yyyy-mm-dd): ");

            var command = new CreateSavingGoalCommand
            {
                BudgetContainerId = activeBudgetId,
                GoalName = goalName,
                TargetAmount = targetAmount,
                CurrentAmount = currentAmount,
                TargetDate = targetDate
            };

            var result = await savingGoalsService.CreateSavingGoalAsync(command);
            Console.WriteLine($"Saving Goal '{result.GoalName}' created successfully with ID: {result.Id}");
        }

        public static async Task ViewSavingGoals(ISavingGoalsService savingGoalsService, IBudgetService budgetService)
        {
            Console.Clear();
            Console.WriteLine("=== View Saving Goals ===");

            var selector = new BudgetSelector(budgetService);
            Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();

            if (activeBudgetId == Guid.Empty)
            {
                Console.WriteLine("No active budget found. Please create a budget first.");
                return;

            }
            var goals = await savingGoalsService.GetSavingGoalsByBudgetContainerIdAsync(activeBudgetId);
            foreach (var goal in goals)
            {
                Console.WriteLine($"ID: {goal.Id} | Name: {goal.GoalName} | Target: {goal.TargetAmount} | " +
                    $"Current: {goal.CurrentAmount} | Target Date: {(goal.TargetDate.HasValue ? goal.TargetDate.Value.ToShortDateString() : "N/A")}");
            }
            if (!goals.Any())
                Console.WriteLine("No saving goals found.");
        }

        public static async Task UpdateSavingGoal(ISavingGoalsService savingGoalsService, InputProcessor inputProcessor, IBudgetService budgetService)
        {
            Console.Clear();
            Console.WriteLine("=== Update Saving Goal ===");

            var selector = new BudgetSelector(budgetService);
            Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();

            if (activeBudgetId == Guid.Empty)
            {
                Console.WriteLine("No active budget found. Please create a budget first.");
                return;
            }

            Console.Write("Enter saving goal ID to update: ");
            if (Guid.TryParse(Console.ReadLine(), out var id))
            {
                string goalName = inputProcessor.GetInput("Enter updated saving goal name: ");
                decimal targetAmount = inputProcessor.GetValidDecimal("Enter updated target amount: ");
                decimal currentAmount = inputProcessor.GetValidDecimal("Enter updated current amount: ");
                DateTime? targetDate = inputProcessor.GetValidDate("Enter updated target date (yyyy-mm-dd): ");

                var command = new UpdateSavingGoalCommand
                {
                    BudgetContainerId = activeBudgetId,
                    Id = id,
                    GoalName = goalName,
                    TargetAmount = targetAmount,
                    CurrentAmount = currentAmount,
                    TargetDate = targetDate
                };

                bool result = await savingGoalsService.UpdateSavingGoalAsync(command);
                Console.WriteLine(result ? "Saving goal updated successfully." : "Saving goal update failed.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        public static async Task DeleteSavingGoal(ISavingGoalsService savingGoalsService, InputProcessor inputProcessor, IBudgetService budgetService)
        {
            Console.Clear();
            Console.WriteLine("=== Delete Saving Goal ===");

            var selector = new BudgetSelector(budgetService);
            Guid activeBudgetId = await selector.GetActiveBudgetContainerIdAsync();

            if (activeBudgetId == Guid.Empty)
            {
                Console.WriteLine("No active budget found. Please create a budget first.");
                return;
            }

            Console.Write("Enter saving goal ID to delete: ");
            if (Guid.TryParse(Console.ReadLine(), out var id))
            {
                bool result = await savingGoalsService.DeleteSavingGoalAsync(id);
                Console.WriteLine(result ? "Saving goal deleted successfully." : "Saving goal deletion failed.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }
    }
}
