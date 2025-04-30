using System;

namespace BudgetTracker.Presentation.UIHelpers
{
    public class Menu
    {
        private readonly IConsole _console;
        
        public Menu(IConsole console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }
        public void DisplayMainMenu()
        {
            Console.Clear(); // Implement this console command??
            _console.WriteLine("=== Budget Tracker ===");
            _console.WriteLine("1. Create Expense");
            _console.WriteLine("2. View Expenses");
            _console.WriteLine("3. Update Expense");
            _console.WriteLine("4. Delete Expense");
            _console.WriteLine("5. Create Budget");
            _console.WriteLine("6. View Budgets");
            _console.WriteLine("7. Update Budget");
            _console.WriteLine("8. Delete Budget");
            _console.WriteLine("9. Create Saving Goal");
            _console.WriteLine("10. View Saving Goals");
            _console.WriteLine("11. Update Saving Goal");
            _console.WriteLine("12. Delete Saving Goal");
            _console.WriteLine("13. Create Income");
            _console.WriteLine("14. View Income");
            _console.WriteLine("15. Update Income");
            _console.WriteLine("16. Delete Income");
            _console.WriteLine("17. View Income Report");
            _console.WriteLine("18. View Detailed Expense Report");
            _console.WriteLine("19. View Saving Goals Report");
            _console.WriteLine("20. View Budget Rule Report");
            _console.WriteLine("21. View Dashboard Summary");
            _console.WriteLine("22. Drill-Down Expense Report");
            _console.WriteLine("23. View Budget Matrix Report");
            _console.WriteLine("24. Exit");
            _console.Write("Enter your choice: ");
        }
    }
}
