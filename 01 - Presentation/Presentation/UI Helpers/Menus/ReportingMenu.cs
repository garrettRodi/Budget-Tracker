using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Presentation.ReportingHelpers;

namespace BudgetTracker.Presentation.UIHelpers
{
    public class ReportingMenu
    {
        private readonly IncomeReportingHelpers _incomeReportingHelpers;
        private readonly ExpenseReportHelpers _expenseReportHelpers;
        private readonly SavingGoalsReportingHelpers _savingGoalsReportingHelpers;
        private readonly BudgetReportingHelpers _budgetReportingHelpers;
        private readonly ReportDashboard _reportDashboard;
        private readonly DrillDownReport _drillDownReport;
        private readonly InputProcessor _input;
        private readonly IConsole _console;
        public ReportingMenu(
            IncomeReportingHelpers incomeReportingHelpers,
            ExpenseReportHelpers expenseReportHelpers,
            SavingGoalsReportingHelpers savingGoalsReportingHelpers,
            BudgetReportingHelpers budgetReportingHelpers,
            ReportDashboard reportDashboard,
            DrillDownReport drillDownReport,
            InputProcessor input,
            IConsole console)
        {
            _incomeReportingHelpers = incomeReportingHelpers
                ?? throw new ArgumentNullException(nameof(incomeReportingHelpers));
            _expenseReportHelpers = expenseReportHelpers
                ?? throw new ArgumentNullException(nameof(expenseReportHelpers));
            _savingGoalsReportingHelpers = savingGoalsReportingHelpers
                ?? throw new ArgumentNullException(nameof(savingGoalsReportingHelpers));
            _budgetReportingHelpers = budgetReportingHelpers
                ?? throw new ArgumentNullException(nameof(budgetReportingHelpers));
            _reportDashboard = reportDashboard
                ?? throw new ArgumentNullException(nameof(reportDashboard));
            _drillDownReport = drillDownReport
                ?? throw new ArgumentNullException(nameof(drillDownReport));
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }
        public async Task ShowAsync()
        {
            bool back = false;
            while (!back)
            {
                _console.Clear();
                _console.WriteLine("=== Reporting Menu ===");
                _console.WriteLine("1. Generate Income Report");
                _console.WriteLine("2. Generate Expense Report");
                _console.WriteLine("3. Generate Savings Report");
                _console.WriteLine("4. Generate Budget Rule Report");
                _console.WriteLine("5. View Report Dashboard");
                _console.WriteLine("6. View Drill Down Report");
                _console.WriteLine("7. View Budget Matrix Report");
                _console.WriteLine("8. Back to Main Menu");
                _console.Write("Choice: ");
                var choice = (_console.ReadLine() ?? "").Trim();
                switch (choice)
                {
                    case "1": await _incomeReportingHelpers.ViewIncomeReportAsync(); break;
                    case "2": await _expenseReportHelpers.ViewExpenseReportAsync(); break;
                    case "3": await _savingGoalsReportingHelpers.ViewSavingGoalsReportAsync(); break;
                    case "4": await _budgetReportingHelpers.ViewBudgetRuleReportAsync(); break;
                    case "5": await _reportDashboard.ViewDashboardAsync(); break;
                    case "6": await _drillDownReport.ViewDrillDownReportAsync(); break;
                    case "7": await _budgetReportingHelpers.ViewBudgetMatrixReportAsync(); break;
                    case "8": back = true; break;
                    default:
                        _console.WriteLine("Invalid option. Press Enter to retry.");
                        _console.ReadLine();
                        break;
                }
            }
        }
    }
}
