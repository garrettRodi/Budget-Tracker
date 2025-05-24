using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Presentation.PresentationHelpers;

namespace BudgetTracker.Presentation.UIHelpers
{
    public class ExpenseMenu
    {
        private readonly ExpenseHelpers _expenseHelpers;
        private readonly PlannedExpenseHelpers _plannedExpenseHelpers;
        private readonly IConsole _console;
        private readonly InputProcessor _input;

        public ExpenseMenu(
            ExpenseHelpers expenseHelpers,
            PlannedExpenseHelpers plannedExpenseHelpers,
            InputProcessor input,
            IConsole console)
        {
            _expenseHelpers = expenseHelpers ?? throw new ArgumentNullException(nameof(expenseHelpers));
            _plannedExpenseHelpers = plannedExpenseHelpers ?? throw new ArgumentNullException(nameof(plannedExpenseHelpers));
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public async Task ShowAsync()
        {
            bool back = false;
            while (!back)
            {
                _console.Clear();
                _console.WriteLine("=== Expense Menu ===");
                _console.WriteLine("1. Create Actual Expense");
                _console.WriteLine("2. View Actual Expenses");
                _console.WriteLine("3. Update Actual Expense");
                _console.WriteLine("4. Delete Actual Expense");
                _console.WriteLine("5. Create Planned Expense");
                _console.WriteLine("6. View Planned Expenses");
                _console.WriteLine("7. Update Planned Expense");
                _console.WriteLine("8. Delete Planned Expense");
                _console.WriteLine("9. Back");
                _console.Write("Choice: ");

                var choice = (_console.ReadLine() ?? "").Trim();

                switch (choice)
                {
                    case "1":
                        await _expenseHelpers.CreateExpenseAsync();
                        break;
                    case "2":
                        await _expenseHelpers.ViewExpensesAsync();
                        break;
                    case "3":
                        await _expenseHelpers.UpdateExpenseAsync();
                        break;
                    case "4":
                        await _expenseHelpers.DeleteExpenseAsync();
                        break;

                    case "5":
                        await _plannedExpenseHelpers.CreatePlannedExpenseAsync();
                        break;
                    case "6":
                        await _plannedExpenseHelpers.ViewPlannedExpensesAsync();
                        break;
                    case "7":
                        await _plannedExpenseHelpers.UpdatePlannedExpenseAsync();
                        break;
                    case "8":
                        await _plannedExpenseHelpers.DeletePlannedExpenseAsync();
                        break;

                    case "9":
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
