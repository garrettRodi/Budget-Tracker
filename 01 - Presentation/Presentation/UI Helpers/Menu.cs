using System;

namespace BudgetTracker.Presentation.UIHelpers
{
    public class Menu
    {
        public void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Budget Tracker ===");
            Console.WriteLine("1. Create Expense");
            Console.WriteLine("2. View Expenses");
            Console.WriteLine("3. Update Expense");
            Console.WriteLine("4. Delete Expense");
            Console.WriteLine("5. Create Budget");
            Console.WriteLine("6. View Budgets");
            Console.WriteLine("7. Update Budget");
            Console.WriteLine("8. Delete Budget");
            Console.WriteLine("9. Create Saving Goal");
            Console.WriteLine("10. View Saving Goals");
            Console.WriteLine("11. Update Saving Goal");
            Console.WriteLine("12. Delete Saving Goal");
            Console.WriteLine("13. Exit");
            Console.Write("Enter your choice: ");
        }
    }
}
