// File: Presentation/AppController.cs
using System;
using System.Threading.Tasks;
using BudgetTracker.Presentation.UIHelpers;
using BudgetTracker.Presentation.ReportingHelpers;
using BudgetTracker.Presentation.PresentationHelpers;

namespace BudgetTracker.Presentation
{
    public class AppController
    {
        private readonly IConsole _console;
        private readonly MainMenu _mainMenu;
        private readonly IncomeMenu _incomeMenu;
        private readonly ExpenseMenu _expenseMenu;
        private readonly BudgetMenu _budgetMenu;
        private readonly ReportingMenu _reportingMenu;
        private readonly SavingGoalsMenu _savingGoalsMenu;
        private readonly ExpenseHelpers _expenseHelpers;
        private readonly BudgetHelpers _budgetHelpers;
        private readonly SavingGoalsHelpers _savingGoalsHelpers;
        private readonly IncomeHelpers _incomeHelpers;
        private readonly BudgetReportingHelpers _budgetReportingHelpers;
        private readonly DrillDownReport _drillDownReport;
        private readonly ExpenseReportHelpers _expenseReportHelpers;
        private readonly IncomeReportingHelpers _incomeReportingHelpers;
        private readonly ReportDashboard _reportDashboard;
        private readonly SavingGoalsReportingHelpers _savingGoalsReportingHelpers;

        public AppController(
            IConsole console,
            MainMenu menu,
            IncomeMenu incomeMenu,
            ExpenseMenu expenseMenu,
            BudgetMenu budgetMenu,
            ReportingMenu reportingMenu,
            SavingGoalsMenu savingGoalsMenu,
            ExpenseHelpers expenseHelpers,
            BudgetHelpers budgetHelpers,
            SavingGoalsHelpers savingGoalsHelpers,
            IncomeHelpers incomeHelpers,
            BudgetReportingHelpers budgetReportingHelpers,
            DrillDownReport drillDownReport,
            ExpenseReportHelpers expenseReportHelpers,
            IncomeReportingHelpers incomeReportingHelpers,
            ReportDashboard reportDashboard,
            SavingGoalsReportingHelpers savingGoalsReportingHelpers
        )
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _mainMenu = menu ?? throw new ArgumentNullException(nameof(menu));
            _incomeMenu = incomeMenu ?? throw new ArgumentNullException(nameof(incomeMenu));
            _expenseMenu = expenseMenu ?? throw new ArgumentNullException(nameof(expenseMenu));
            _budgetMenu = budgetMenu ?? throw new ArgumentNullException(nameof(budgetMenu));
            _reportingMenu = reportingMenu ?? throw new ArgumentNullException(nameof(reportingMenu));
            _savingGoalsMenu = savingGoalsMenu ?? throw new ArgumentNullException(nameof(savingGoalsMenu));
            _expenseHelpers = expenseHelpers ?? throw new ArgumentNullException(nameof(expenseHelpers));
            _budgetHelpers = budgetHelpers ?? throw new ArgumentNullException(nameof(budgetHelpers));
            _savingGoalsHelpers = savingGoalsHelpers ?? throw new ArgumentNullException(nameof(savingGoalsHelpers));
            _incomeHelpers = incomeHelpers ?? throw new ArgumentNullException(nameof(incomeHelpers));
            _budgetReportingHelpers = budgetReportingHelpers ?? throw new ArgumentNullException(nameof(budgetReportingHelpers));
            _drillDownReport = drillDownReport ?? throw new ArgumentNullException(nameof(drillDownReport));
            _expenseReportHelpers = expenseReportHelpers ?? throw new ArgumentNullException(nameof(expenseReportHelpers));
            _incomeReportingHelpers = incomeReportingHelpers ?? throw new ArgumentNullException(nameof(incomeReportingHelpers));
            _reportDashboard = reportDashboard ?? throw new ArgumentNullException(nameof(reportDashboard));
            _savingGoalsReportingHelpers = savingGoalsReportingHelpers ?? throw new ArgumentNullException(nameof(savingGoalsReportingHelpers));
        }

        public async Task RunAsync()
        {
            bool exitRequested = false;

            Console.WriteLine("DEBUG: Entering RunAsync");

            while (!exitRequested)
            {
                _mainMenu.DisplayMainMenu();
                var choice = (_console.ReadLine() ?? string.Empty).Trim();

                switch (choice)
                {
                    case "1": await _budgetMenu.ShowAsync(); break; // Budget Menu
                    case "2": await _incomeMenu.ShowAsync(); break; // Income Menu
                    case "3": await _expenseMenu.ShowAsync(); break; // Expense Menu
                    case "4": await _savingGoalsMenu.ShowAsync(); break; // Saving Goals Menu
                    case "5": await _reportingMenu.ShowAsync(); break; // Report Menu
                    case "6": 
                        exitRequested = true;
                        Console.WriteLine("DEBUG: Exit requested");
                        break;

                    default:
                        _console.WriteLine("Invalid choice. Please select a valid menu option.");
                        break;
                }

                if (!exitRequested)
                {
                    _console.WriteLine("\nPress Enter to continue...");
                    _console.ReadLine();
                }
            }
        }
    }
}
