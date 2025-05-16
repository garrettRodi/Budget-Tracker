// File: 01 - Presentation/UIHelpers/IncomeMenu.cs
using System;
using System.Threading.Tasks;
using BudgetTracker.Presentation.UIHelpers;
using BudgetTracker.Presentation.PresentationHelpers;

namespace BudgetTracker.Presentation.UIHelpers
{
    public class IncomeMenu
    {
        private readonly IncomeHelpers _actualHelpers;
        private readonly PlannedIncomeHelpers _plannedHelpers;
        private readonly IConsole _console;

        public IncomeMenu(
            IncomeHelpers actualHelpers,
            PlannedIncomeHelpers plannedHelpers,
            IConsole console)
        {
            _actualHelpers = actualHelpers ?? throw new ArgumentNullException(nameof(actualHelpers));
            _plannedHelpers = plannedHelpers ?? throw new ArgumentNullException(nameof(plannedHelpers));
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public async Task ShowAsync()
        {
            bool back = false;
            while (!back)
            {
                _console.Clear();
                _console.WriteLine("=== Income Menu ===");
                _console.WriteLine("1. Create Actual Income");
                _console.WriteLine("2. Create Planned Income");
                _console.WriteLine("3. View Actual Incomes");
                _console.WriteLine("4. View Planned Incomes");
                _console.WriteLine("5. Update Actual Income");
                _console.WriteLine("6. Update Planned Income");
                _console.WriteLine("7. Delete Actual Income");
                _console.WriteLine("8. Delete Planned Income");
                _console.WriteLine("9. Back to Main Menu");
                _console.Write("Choice: ");

                var choice = (_console.ReadLine() ?? string.Empty).Trim();
                switch (choice)
                {
                    case "1": await _actualHelpers.CreateIncomeAsync(); break;
                    case "2": await _plannedHelpers.CreatePlannedIncomeAsync(); break;
                    case "3": await _actualHelpers.ViewIncomesAsync(); break;
                    case "4": await _plannedHelpers.ViewPlannedIncomesAsync(); break;
                    case "5": await _actualHelpers.UpdateIncomeAsync(); break;
                    case "6": await _plannedHelpers.UpdatePlannedIncomeAsync(); break;
                    case "7": await _actualHelpers.DeleteIncomeAsync(); break;
                    case "8": await _plannedHelpers.DeletePlannedIncomeAsync(); break;
                    case "9": back = true; break;
                    default:
                        _console.WriteLine("Invalid option. Press Enter to retry.");
                        _console.ReadLine();
                        break;
                }
            }
        }
    }
}
