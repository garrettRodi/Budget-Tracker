using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BudgetTracker.Infrastructure.DataAccess;
using BudgetTracker.Infrastructure.RepositoryImplementations;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.ExternalServices;
using BudgetTracker.Presentation.UIHelpers;
using BudgetTracker.Application.Services;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Infrastructure.Logging;
using BudgetTracker.Infrastructure.Interface;
using BudgetTracker.Application.DTOs.Commands;

Console.WriteLine("Welcome to the Budget Tracker!");

// Create the host builder to set up dependency injection
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register DbContext with EF Core using SQLite.
        services.AddDbContext<BudgetTrackerDbContext>(options =>
            options.UseSqlite("Data Source=BudgetTracker.db",
                b => b.MigrationsAssembly("04 - Infrastructure")));

        // Register repository implementations.
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IIncomeRepository, IncomeRepository>();
        services.AddScoped<IBudgetRepository, BudgetRepository>();
        services.AddScoped<ISavingGoalsRepository, SavingGoalsRepository>();
        services.AddScoped<ICategoryMappingRepository, CategoryMappingRepository>();

        // Register application services.
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IBudgetService, BudgetService>();
        services.AddScoped<IIncomeService, IncomeService>();
        services.AddScoped<ISavingGoalsService, SavingGoalsService>();
        services.AddScoped<IReportingService, ReportingService>();
        services.AddScoped<ICategoryMappingService, CategoryMappingService>();

        // Register ExpenseValidator.
        services.AddScoped<BudgetTracker.Domain.Services.ExpenseValidator>();

        // Register UI Helpers.
        services.AddScoped<Menu>();
        services.AddScoped<InputProcessor>();

        // Register HTTPClient and the CurrencyConversionService.
        services.AddHttpClient<CurrencyConversionService>();

        // Register a FileLogger with a fixed log file path.
        services.AddScoped<IBudgetLogger>(sp => new FileLogger("log.txt"));

        // Register UnitOfWork.
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    })
    .Build();

// Create a DI scope.
using var scope = host.Services.CreateScope();
var provider = scope.ServiceProvider;

// Apply pending migrations.
var dbContext = provider.GetRequiredService<BudgetTrackerDbContext>();
dbContext.Database.Migrate();

// Resolve UI Helpers and application services.
var menu = provider.GetRequiredService<Menu>();
var inputProcessor = provider.GetRequiredService<InputProcessor>();
var expenseService = provider.GetRequiredService<IExpenseService>();
var budgetService = provider.GetRequiredService<IBudgetService>();
var savingGoalsService = provider.GetRequiredService<ISavingGoalsService>();
var reportingService = provider.GetRequiredService<IReportingService>();
var logger = provider.GetRequiredService<IBudgetLogger>();

bool exitRequested = false;
while (!exitRequested)
{
    try
    {
        menu.DisplayMainMenu();
        string choice = Console.ReadLine() ?? string.Empty;

        switch (choice)
        {
            case "1":
                await CreateExpense(expenseService);
                break;
            case "2":
                await ViewExpenses(expenseService);
                break;
            case "3":
                await UpdateExpense(expenseService);
                break;
            case "4":
                await DeleteExpense(expenseService);
                break;
            case "5":
                await CreateBudget(budgetService, inputProcessor);
                break;
            case "6":
                await ViewBudgets(budgetService);
                break;
            case "7":
                await UpdateBudget(budgetService, inputProcessor);
                break;
            case "8":
                await DeleteBudget(budgetService, inputProcessor);
                break;
            case "9":
                await CreateSavingGoal(savingGoalsService, inputProcessor);
                break;
            case "10":
                await ViewSavingGoals(savingGoalsService);
                break;
            case "11":
                await UpdateSavingGoal(savingGoalsService, inputProcessor);
                break;
            case "12":
                await DeleteSavingGoal(savingGoalsService, inputProcessor);
                break;
            // Reporting Options:
            case "14":
                await GenerateExpenseReport(reportingService, inputProcessor);
                break;
            case "15":
                await GenerateBudgetReport(reportingService);
                break;
            case "16":
                await GenerateIncomeReport(reportingService, inputProcessor);
                break;
            case "17":
                await GenerateBudgetRuleReport(reportingService, inputProcessor);
                break;
            case "13":
                exitRequested = true;
                break;
            default:
                Console.WriteLine("Invalid choice. Try again.");
                break;
        }
    }
    catch (ApplicationException appEx)
    {
        Console.WriteLine("Error: " + appEx.Message);
        logger.Log($"Application error: {appEx.Message}\n{appEx.StackTrace}");
    }
    catch (Exception ex)
    {
        logger.Log($"Exception: {ex.Message}\n{ex.StackTrace}");
        Console.WriteLine("An error occurred. Please check the log file for details.");
    }

    if (!exitRequested)
    {
        Console.WriteLine("Press any key to return to the menu...");
        Console.ReadKey();
    }
}

Console.WriteLine("Exiting application. Press any key to close.");
Console.ReadKey();


// ----------------------- Expense Helper Methods -----------------------

static async Task CreateExpense(IExpenseService expenseService)
{
    Console.Clear();
    Console.WriteLine("=== Create Expense ===");

    Console.Write("Enter expense name: ");
    string name = Console.ReadLine() ?? string.Empty;

    Console.Write("Enter amount: ");
    decimal amount = decimal.TryParse(Console.ReadLine(), out var amt) ? amt : 0m;

    Console.Write("Enter expense date (yyyy-mm-dd): ");
    DateTime date = DateTime.TryParse(Console.ReadLine(), out var expDate) ? expDate : DateTime.Now;

    Console.Write("Enter category: ");
    string category = Console.ReadLine() ?? string.Empty;

    var command = new BudgetTracker.Application.DTOs.Commands.CreateExpenseCommand
    {
        Name = name,
        Amount = amount,
        Date = date,
        Category = category
    };

    var expenseDto = await expenseService.CreateExpenseAsync(command);
    Console.WriteLine($"Expense '{expenseDto.Name}' created successfully with ID: {expenseDto.Id}");
}

static async Task ViewExpenses(IExpenseService expenseService)
{
    Console.Clear();
    Console.WriteLine("=== View Expenses ===");
    var expenses = await expenseService.GetExpenseAsync();
    foreach (var expense in expenses)
    {
        Console.WriteLine($"ID: {expense.Id} | Name: {expense.Name} | Amount: {expense.Amount} | Date: {expense.Date:d} | Category: {expense.Category}");
    }
    if (!expenses.Any())
        Console.WriteLine("No expenses found.");
}

static async Task UpdateExpense(IExpenseService expenseService)
{
    Console.Clear();
    Console.WriteLine("=== Update Expense ===");
    Console.Write("Enter expense ID to update: ");
    if (Guid.TryParse(Console.ReadLine(), out var id))
    {
        Console.Write("Enter new expense name: ");
        string name = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter new amount: ");
        decimal amount = decimal.TryParse(Console.ReadLine(), out var amt) ? amt : 0m;

        Console.Write("Enter new expense date (yyyy-mm-dd): ");
        DateTime date = DateTime.TryParse(Console.ReadLine(), out var expDate) ? expDate : DateTime.Now;

        Console.Write("Enter new category: ");
        string category = Console.ReadLine() ?? string.Empty;

        var updateCommand = new BudgetTracker.Application.DTOs.Commands.UpdateExpenseCommand
        {
            Id = id,
            Name = name,
            Amount = amount,
            Date = date,
            Category = category
        };

        bool result = await expenseService.UpdateExpenseAsync(updateCommand);
        Console.WriteLine(result ? "Expense updated successfully." : "Expense update failed.");
    }
    else
    {
        Console.WriteLine("Invalid ID format.");
    }
}

static async Task DeleteExpense(IExpenseService expenseService)
{
    Console.Clear();
    Console.WriteLine("=== Delete Expense ===");
    Console.Write("Enter expense ID to delete: ");
    if (Guid.TryParse(Console.ReadLine(), out var id))
    {
        bool result = await expenseService.DeleteExpenseAsync(id);
        Console.WriteLine(result ? "Expense deleted successfully." : "Expense deletion failed.");
    }
    else
    {
        Console.WriteLine("Invalid ID format.");
    }
}


// ----------------------- Budget Helper Methods -----------------------

static async Task CreateBudget(IBudgetService budgetService, InputProcessor inputProcessor)
{
    Console.Clear();
    Console.WriteLine("=== Create Budget ===");

    Console.Write("Enter budget name: ");
    string name = Console.ReadLine() ?? string.Empty;

    Console.Write("Enter budget frequency (Weekly/Monthly/Yearly): ");
    string frequencyInput = Console.ReadLine() ?? "Monthly";
    var frequency = Enum.TryParse(frequencyInput, true, out BudgetTracker.Domain.Entities.BudgetFrequency freq)
        ? freq : BudgetTracker.Domain.Entities.BudgetFrequency.Monthly;

    Console.Write("Enter start date (yyyy-mm-dd): ");
    DateTime startDate = DateTime.TryParse(Console.ReadLine(), out var sDate) ? sDate : DateTime.Now;

    Console.Write("Enter end date (yyyy-mm-dd): ");
    DateTime endDate = DateTime.TryParse(Console.ReadLine(), out var eDate) ? eDate : DateTime.Now.AddMonths(1);

    Console.Write("Auto renew? (y/n): ");
    bool autoRenew = (Console.ReadLine()?.ToLower() == "y");

    // Collect budget items
    Console.WriteLine("How many budget items do you want to add??");
    int itemCount = int.TryParse(Console.ReadLine(), out int count) ? count : 0;
    var budgetItems = new List<BudgetTracker.Application.DTOs.Commands.CreateBudgetItemCommand>();

    for (int i = 0; i < itemCount; i++)
    {
        Console.WriteLine($"--- Budget Item {i + 1} ---");
        Console.Write("Enter category (e.g., Food, Rent, Savings, Entertainment): ");
        string category = Console.ReadLine() ?? string.Empty;
        Console.Write("Enter planned amount: ");
        decimal plannedAmount = decimal.TryParse(Console.ReadLine(), out var amt) ? amt : 0m;

        var itemCommand = new BudgetTracker.Application.DTOs.Commands.CreateBudgetItemCommand
        {
            Category = category,
            PlannedAmount = plannedAmount
        };
        budgetItems.Add(itemCommand);
    }

    var createCommand = new CreateBudgetCommand
    {
        Name = name,
        Frequency = frequency,
        StartDate = startDate,
        EndDate = endDate,
        AutoRenew = autoRenew
    };

    var result = await budgetService.CreateBudgetAsync(createCommand);
    Console.WriteLine($"Budget '{result.Name}' created successfully with ID: {result.Id}");
}

static async Task ViewBudgets(IBudgetService budgetService)
{
    Console.Clear();
    Console.WriteLine("=== View Budgets ===");
    var budgets = await budgetService.GetAllBudgetsAsync();
    foreach (var budget in budgets)
    {
        Console.WriteLine($"ID: {budget.Id} | Name: {budget.Name} | Frequency: {budget.Frequency} | " +
            $"Start: {budget.StartDate:d} | End: {budget.EndDate:d} | AutoRenew: {budget.AutoRenew}");
    }
    if (!budgets.Any())
        Console.WriteLine("No budgets found.");
}

static async Task UpdateBudget(IBudgetService budgetService, InputProcessor inputProcessor)
{
    Console.Clear();
    Console.WriteLine("=== Update Budget ===");
    Console.Write("Enter budget ID to update: ");
    if (Guid.TryParse(Console.ReadLine(), out var id))
    {
        Console.Write("Enter new budget name: ");
        string name = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter new frequency (Weekly/Monthly/Yearly): ");
        string frequencyInput = Console.ReadLine() ?? "Monthly";
        var frequency = Enum.TryParse(frequencyInput, true, out BudgetTracker.Domain.Entities.BudgetFrequency freq)
            ? freq : BudgetTracker.Domain.Entities.BudgetFrequency.Monthly;

        Console.Write("Enter new start date (yyyy-mm-dd): ");
        DateTime startDate = DateTime.TryParse(Console.ReadLine(), out var sDate) ? sDate : DateTime.Now;

        Console.Write("Enter new end date (yyyy-mm-dd): ");
        DateTime endDate = DateTime.TryParse(Console.ReadLine(), out var eDate) ? eDate : DateTime.Now.AddMonths(1);

        Console.Write("Auto renew? (y/n): ");
        bool autoRenew = (Console.ReadLine()?.ToLower() == "y");

        var updateCommand = new UpdateBudgetCommand
        {
            Id = id,
            Name = name,
            Frequency = frequency,
            StartDate = startDate,
            EndDate = endDate,
            AutoRenew = autoRenew
        };

        bool result = await budgetService.UpdateBudgetAsync(updateCommand);
        Console.WriteLine(result ? "Budget updated successfully." : "Budget update failed.");
    }
    else
    {
        Console.WriteLine("Invalid ID format.");
    }
}

static async Task DeleteBudget(IBudgetService budgetService, InputProcessor inputProcessor)
{
    Console.Clear();
    Console.WriteLine("=== Delete Budget ===");
    Console.Write("Enter budget ID to delete: ");
    if (Guid.TryParse(Console.ReadLine(), out var id))
    {
        bool result = await budgetService.DeleteBudgetAsync(id);
        Console.WriteLine(result ? "Budget deleted successfully." : "Budget deletion failed.");
    }
    else
    {
        Console.WriteLine("Invalid ID format.");
    }
}


// ----------------------- Saving Goals Helper Methods -----------------------

static async Task CreateSavingGoal(ISavingGoalsService savingGoalsService, InputProcessor inputProcessor)
{
    Console.Clear();
    Console.WriteLine("=== Create Saving Goal ===");

    Console.Write("Enter goal name: ");
    string goalName = Console.ReadLine() ?? string.Empty;

    Console.Write("Enter target amount: ");
    decimal targetAmount = decimal.TryParse(Console.ReadLine(), out var tAmount) ? tAmount : 0m;

    Console.Write("Enter current amount: ");
    decimal currentAmount = decimal.TryParse(Console.ReadLine(), out var cAmount) ? cAmount : 0m;

    Console.Write("Enter target date (yyyy-mm-dd) or leave blank: ");
    DateTime? targetDate = DateTime.TryParse(Console.ReadLine(), out var tDate) ? tDate : (DateTime?)null;

    var command = new CreateSavingGoalCommand
    {
        GoalName = goalName,
        TargetAmount = targetAmount,
        CurrentAmount = currentAmount,
        TargetDate = targetDate
    };

    var result = await savingGoalsService.CreateSavingGoalAsync(command);
    Console.WriteLine($"Saving Goal '{result.GoalName}' created successfully with ID: {result.Id}");
}

static async Task ViewSavingGoals(ISavingGoalsService savingGoalsService)
{
    Console.Clear();
    Console.WriteLine("=== View Saving Goals ===");
    var goals = await savingGoalsService.GetAllSavingGoalsAsync();
    foreach (var goal in goals)
    {
        Console.WriteLine($"ID: {goal.Id} | Name: {goal.GoalName} | Target: {goal.TargetAmount} | " +
            $"Current: {goal.CurrentAmount} | Target Date: {(goal.TargetDate.HasValue ? goal.TargetDate.Value.ToShortDateString() : "N/A")}");
    }
    if (!goals.Any())
        Console.WriteLine("No saving goals found.");
}

static async Task UpdateSavingGoal(ISavingGoalsService savingGoalsService, InputProcessor inputProcessor)
{
    Console.Clear();
    Console.WriteLine("=== Update Saving Goal ===");
    Console.Write("Enter saving goal ID to update: ");
    if (Guid.TryParse(Console.ReadLine(), out var id))
    {
        Console.Write("Enter new goal name: ");
        string goalName = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter new target amount: ");
        decimal targetAmount = decimal.TryParse(Console.ReadLine(), out var tAmount) ? tAmount : 0m;

        Console.Write("Enter new current amount: ");
        decimal currentAmount = decimal.TryParse(Console.ReadLine(), out var cAmount) ? cAmount : 0m;

        Console.Write("Enter new target date (yyyy-mm-dd) or leave blank: ");
        DateTime? targetDate = DateTime.TryParse(Console.ReadLine(), out var tDate) ? tDate : (DateTime?)null;

        var command = new UpdateSavingGoalCommand
        {
            Id = id,
            GoalName = goalName,
            TargetAmount = targetAmount,
            CurrentAmount = currentAmount,
            TargetDate = targetDate
        };

        bool result = await savingGoalsService.UpdateSavingGoalAsync(command);
        Console.WriteLine(result ? "Saving goal updated successfully." : "Saving goal update failed.");
    }
    else
    {
        Console.WriteLine("Invalid ID format.");
    }
}

static async Task DeleteSavingGoal(ISavingGoalsService savingGoalsService, InputProcessor inputProcessor)
{
    Console.Clear();
    Console.WriteLine("=== Delete Saving Goal ===");
    Console.Write("Enter saving goal ID to delete: ");
    if (Guid.TryParse(Console.ReadLine(), out var id))
    {
        bool result = await savingGoalsService.DeleteSavingGoalAsync(id);
        Console.WriteLine(result ? "Saving goal deleted successfully." : "Saving goal deletion failed.");
    }
    else
    {
        Console.WriteLine("Invalid ID format.");
    }
}


// ----------------------- Reporting Helper Methods -----------------------

static async Task GenerateExpenseReport(IReportingService reportingService, InputProcessor inputProcessor)
{
    Console.Clear();
    Console.WriteLine("=== Enhanced Expense Report ===");
    Console.Write("Enter start date (yyyy-mm-dd): ");
    DateTime startDate = DateTime.TryParse(Console.ReadLine(), out var sDate) ? sDate : DateTime.Now.AddDays(-30);
    Console.Write("Enter end date (yyyy-mm-dd): ");
    DateTime endDate = DateTime.TryParse(Console.ReadLine(), out var eDate) ? eDate : DateTime.Now;
    var report = await reportingService.GenerateExpenseReportAsync(startDate, endDate);
    Console.WriteLine($"Report for {report.StartDate:d} to {report.EndDate:d}");
    Console.WriteLine($"Total Expenses: {report.TotalExpenses:C}");
    Console.WriteLine("Category Breakdown (Percentage):");
    foreach (var kvp in report.CategoryPercentages)
    {
        Console.WriteLine($"  {kvp.Key}: {kvp.Value:F2}%");
    }
}

static async Task GenerateBudgetReport(IReportingService reportingService)
{
    Console.Clear();
    Console.WriteLine("=== Budget Report ===");
    var report = await reportingService.GenerateBudgetReportAsync();
    Console.WriteLine($"Planned Budget: {report.BudgetedExpenses:C}");
    Console.WriteLine($"Actual Expenses: {report.ActualExpenses:C}");
    Console.WriteLine($"Difference: {report.Difference:C}");
}

static async Task GenerateIncomeReport(IReportingService reportingService, InputProcessor inputProcessor)
{
    Console.Clear();
    Console.WriteLine("=== Income Report ===");
    Console.Write("Enter start date (yyyy-mm-dd): ");
    DateTime startDate = DateTime.TryParse(Console.ReadLine(), out var sDate) ? sDate : DateTime.Now.AddDays(-30);
    Console.Write("Enter end date (yyyy-mm-dd): ");
    DateTime endDate = DateTime.TryParse(Console.ReadLine(), out var eDate) ? eDate : DateTime.Now;
    var report = await reportingService.GenerateIncomeReportAsync(startDate, endDate);
    Console.WriteLine($"Income Report for {report.StartDate:d} to {report.EndDate:d}");
    Console.WriteLine($"Total Income: {report.TotalIncome:C}");
}

static async Task GenerateBudgetRuleReport(IReportingService reportingService, InputProcessor inputProcessor)
{
    Console.Clear();
    Console.WriteLine("=== Budget Rule Report ===");
    Console.Write("Enter budget rule (e.g., 50/20/30): ");
    string rule = Console.ReadLine() ?? "50/20/30";
    // For demonstration, we'll use the entire timeline.
    var report = await reportingService.GenerateBudgetRuleReportAsync(rule, DateTime.MinValue, DateTime.MaxValue);
    Console.WriteLine($"Budget Rule: {report.Rule}");
    Console.WriteLine($"Necessities - Planned: {report.NecessitiesPlanned:C}, Actual: {report.NecessitiesActual:C}, Variance: {report.NecessitiesPercentageVariance:F2}%");
    Console.WriteLine($"Savings - Planned: {report.SavingsPlanned:C}, Actual: {report.SavingsActual:C}, Variance: {report.SavingsPercentageVariance:F2}%");
    Console.WriteLine($"Discretionary - Planned: {report.DiscretionaryPlanned:C}, Actual: {report.DiscretionaryActual:C}, Variance: {report.DiscretionaryPercentageVariance:F2}%");
}
