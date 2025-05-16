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
            _console.WriteLine("13. Income Management");
            _console.WriteLine("2. Expense Management");
            _console.WriteLine("3. Budget Management");
            _console.WriteLine("4. Saving Goals");
            _console.WriteLine("5. Reporting");
            _console.WriteLine("6. Exit");
            _console.Write("Enter your choice: ");
        }
    }
}
