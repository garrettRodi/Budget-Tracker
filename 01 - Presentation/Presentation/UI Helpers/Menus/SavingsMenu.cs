// File: Presentation/UIHelpers/SavingGoalsMenu.cs
using System;
using System.Threading.Tasks;
using BudgetTracker.Presentation.PresentationHelpers;  // for SavingGoalsHelpers
using BudgetTracker.Presentation.UIHelpers;             // for SelectBudgetContainer, IConsole

namespace BudgetTracker.Presentation.UIHelpers
{
    public class SavingGoalsMenu
    {
        private readonly SavingGoalsHelpers _savingGoalsHelpers;
        private readonly SelectBudgetContainer _selector;
        private readonly IConsole _console;

        public SavingGoalsMenu(
            SavingGoalsHelpers savingGoalsHelpers,
            SelectBudgetContainer selector,
            IConsole console)
        {
            _savingGoalsHelpers = savingGoalsHelpers
                ?? throw new ArgumentNullException(nameof(savingGoalsHelpers));
            _selector = selector
                ?? throw new ArgumentNullException(nameof(selector));
            _console = console
                ?? throw new ArgumentNullException(nameof(console));
        }

        public async Task ShowAsync()
        {
            bool back = false;
            while (!back)
            {
                _console.Clear();
                _console.WriteLine("=== Saving Goals Menu ===");
                _console.WriteLine("1. Create Saving Goal");
                _console.WriteLine("2. View Saving Goals");
                _console.WriteLine("3. Update Saving Goal");
                _console.WriteLine("4. Delete Saving Goal");
                _console.WriteLine("5. Back");
                _console.Write("Choice: ");

                var choice = (_console.ReadLine() ?? "").Trim();
                var budgetId = await _selector.GetActiveBudgetContainerIdAsync();
                if (budgetId == Guid.Empty) return;

                switch (choice)
                {
                    case "1":
                        await _savingGoalsHelpers.CreateSavingGoalAsync();
                        break;
                    case "2":
                        await _savingGoalsHelpers.ViewSavingGoalsAsync();
                        break;
                    case "3":
                        await _savingGoalsHelpers.UpdateSavingGoalAsync();
                        break;
                    case "4":
                        await _savingGoalsHelpers.DeleteSavingGoalAsync();
                        break;
                    case "5":
                        back = true;
                        break;
                    default:
                        _console.WriteLine("Invalid choice.");
                        break;
                }

                if (!back)
                {
                    _console.WriteLine("\nPress Enter to continue...");
                    _console.ReadLine();
                }
            }
        }
    }
}
