using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Presentation.PresentationHelpers;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.UIHelpers
{ 
    public class BudgetMenu
    {
        private readonly BudgetHelpers _budgetHelpers;
        private readonly InputProcessor _input;
        private readonly IConsole _console;
        public BudgetMenu(BudgetHelpers budgetHelper, InputProcessor input, IConsole console)
        {
            _budgetHelpers = budgetHelper
                ?? throw new ArgumentNullException(nameof(budgetHelper));
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }
        public async Task ShowAsync()
        {
            bool back = false;
            while (!back)
            {
                _console.Clear();
                _console.WriteLine("=== Budget Menu ===");
                _console.WriteLine("1. Create Budget");
                _console.WriteLine("2. View Budgets");
                _console.WriteLine("3. Update Budget");
                _console.WriteLine("4. Delete Budget");
                _console.WriteLine("5. Back to Main Menu");
                _console.Write("Choice: ");

                var choice = (_console.ReadLine() ?? "").Trim();

                switch (choice)
                {
                    case "1": await _budgetHelpers.CreateBudgetAsync(); break;
                    case "2": await _budgetHelpers.ViewBudgetsAsync(); break;
                    case "3": await _budgetHelpers.UpdateBudgetAsync(); break;
                    case "4": await _budgetHelpers.DeleteBudgetAsync(); break;
                    case "5": back = true; break;
                    default:
                        _console.WriteLine("Invalid option. Press Enter to retry.");
                        _console.ReadLine();
                        break;
                }
            }
        }
    }
}
