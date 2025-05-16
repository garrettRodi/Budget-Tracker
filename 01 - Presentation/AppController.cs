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
                    case "1": await _expenseHelpers.CreateExpenseAsync(); break;
                    case "2": await _expenseHelpers.ViewExpensesAsync(); break;
                    case "3": await _expenseHelpers.UpdateExpenseAsync(); break;
                    case "4": await _expenseHelpers.DeleteExpenseAsync(); break;

                    case "5": await _budgetHelpers.CreateBudgetAsync(); break;
                    case "6": await _budgetHelpers.ViewBudgetsAsync(); break;
                    case "7": await _budgetHelpers.UpdateBudgetAsync(); break;
                    case "8": await _budgetHelpers.DeleteBudgetAsync(); break;

                    case "9": await _savingGoalsHelpers.CreateSavingGoalAsync(); break;
                    case "10": await _savingGoalsHelpers.ViewSavingGoalsAsync(); break;
                    case "11": await _savingGoalsHelpers.UpdateSavingGoalAsync(); break;
                    case "12": await _savingGoalsHelpers.DeleteSavingGoalAsync(); break;

                    case "13":
                        await _incomeMenu.ShowAsync();
                        break;

                    case "17": await _incomeReportingHelpers.ViewIncomeReportAsync(); break;
                    case "18": await _expenseReportHelpers.ViewExpenseReportAsync(); break;
                    case "19": await _savingGoalsReportingHelpers.ViewSavingGoalsReportAsync(); break;
                    case "20": await _budgetReportingHelpers.ViewBudgetRuleReportAsync(); break;
                    case "21": await _reportDashboard.ViewDashboardAsync(); break;
                    case "22": await _drillDownReport.ViewDrillDownReportAsync(); break;
                    case "23": await _budgetReportingHelpers.ViewBudgetMatrixReportAsync(); break;
                    case "24": 
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
