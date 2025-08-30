using System;

namespace BudgetTracker.Presentation.UIHelpers
{
    public class MainMenu
    {
        private readonly IConsole _console;
        
        public MainMenu(IConsole console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }
        public void DisplayMainMenu()
        {
            _console.Clear();
            _console.WriteLine("=== Budget Tracker ===");
            _console.WriteLine("1. Budget Management");
            _console.WriteLine("2. Income Management");
            _console.WriteLine("3. Expense Management");
            _console.WriteLine("4. Savings Management");
            _console.WriteLine("5. Reporting");
            _console.WriteLine("6. Settings");
            _console.WriteLine("7. Exit");
            _console.Write("Enter your choice: ");
        }
    }
}
