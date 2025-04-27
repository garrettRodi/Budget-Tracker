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
            Console.WriteLine("13. Create Income");
            Console.WriteLine("14. View Income");
            Console.WriteLine("15. Update Income");
            Console.WriteLine("16. Delete Income");
            Console.WriteLine("17. View Income Report");
            Console.WriteLine("18. View Detailed Expense Report");
            Console.WriteLine("19. View Saving Goals Report");
            Console.WriteLine("20. View Budget Rule Report");
            Console.WriteLine("21. View Dashboard Summary");
            Console.WriteLine("22. Drill-Down Expense Report");
            Console.WriteLine("23. View Budget Matrix Report");
            Console.WriteLine("24. Exit");
            Console.Write("Enter your choice: ");
        }
    }
}
