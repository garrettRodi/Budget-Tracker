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
using BudgetTracker.Presentation.ReportingHelpers;

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
var incomeService = provider.GetRequiredService<IIncomeService>();
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
                await CreateExpense(expenseService, inputProcessor);
                break;
            case "2":
                await ViewExpenses(expenseService);
                break;
            case "3":
                await UpdateExpense(expenseService, inputProcessor);
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
            case "13":
                await CreateIncome(incomeService, inputProcessor);
                break;
            // Reporting Options:
            case "14":
                await IncomeReportingHelpers.ViewIncomeReport(reportingService, inputProcessor);
                break;
            case "15":
                await BudgetReportingHelpers.ViewBudgetReport(reportingService);
                break;
            case "16":
                await ExpenseReportHelpers.ViewExpenseReport(reportingService, inputProcessor);
                break;
            case "17":
                await IncomeReportingHelpers.ViewIncomeReport(reportingService, inputProcessor);
                break;
            case "18":
                await SavingGoalsReportingHelpers.ViewSavingGoalsReport(reportingService);
                break;
            case "19":
                await BudgetReportingHelpers.ViewBudgetRuleReport(reportingService, inputProcessor);
                break;
            case "20":
                await ReportDashboard.ViewDashboard(reportingService);
                break;
            case "21":
                await DrillDown.DrillDownReport(reportingService, inputProcessor);
                break;
            case "22":
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

// ----------------------- Income Helper Methods -----------------------

static async Task CreateIncome(IIncomeService incomeService, InputProcessor inputProcessor)
{
    Console.Clear();
    Console.WriteLine("=== Create Income ===");
    string source = inputProcessor.GetInput("Enter income source (i.e., Salary, Bonus): ");
     
    decimal amount = inputProcessor.GetValidDecimal("Enter the actual amount recieved: ");

    DateTime date = inputProcessor.GetValidDate("Enter recieved date (yyyy-mm-dd): ");
    
    var command = new BudgetTracker.Application.DTOs.Commands.CreateIncomeCommand
    {
        Source = source,
        ActualAmount = amount,
        ReceivedDate = date
    };
    var incomeDto = await incomeService.CreateIncomeAsync(command);
    Console.WriteLine($"Income from '{incomeDto.Source}' created successfully with ID: {incomeDto.Id}");
}

static async Task ViewIncomes(IIncomeService incomeService)
{
    Console.Clear();
    Console.WriteLine("=== View Incomes ===");
    var incomes = await incomeService.GetAllIncomesAsync();
    foreach (var income in incomes)
    {
        Console.WriteLine($"ID: {income.Id} | Source: {income.Source} | Amount: {income.ActualAmount} | Date: {income.ReceivedDate:d}");
    }
    if (!incomes.Any())
        Console.WriteLine("No incomes found.");
}

static async Task UpdateIncome(IIncomeService incomeService, InputProcessor inputProcessor)
{
    Console.Clear();
    Console.WriteLine("=== Update Income ===");
    Console.Write("Enter income ID to update: ");
    if (Guid.TryParse(Console.ReadLine(), out var id))
    {
        string source = inputProcessor.GetInput("Enter updated income source: ");
        decimal amount = inputProcessor.GetValidDecimal("Enter updated actual amount recieved: ");
        DateTime date = inputProcessor.GetValidDate("Enter updated recieved date (yyyy-mm-dd): ");
        var updateCommand = new BudgetTracker.Application.DTOs.Commands.UpdateIncomeCommand
        {
            Id = id,
            Source = source,
            ActualAmount = amount,
            ReceivedDate = date
        };
        bool result = await incomeService.UpdateIncomeAsync(updateCommand);
        Console.WriteLine(result ? "Income updated successfully." : "Income update failed.");
    }
    else
    {
        Console.WriteLine("Invalid ID format.");
    }
}

static async Task DeleteIncome(IIncomeService incomeService)
{
    Console.Clear();
    Console.WriteLine("=== Delete Income ===");
    Console.Write("Enter income ID to delete: ");
    if (Guid.TryParse(Console.ReadLine(), out var id))
    {
        bool result = await incomeService.DeleteIncomeAsync(id);
        Console.WriteLine(result ? "Income deleted successfully." : "Income deletion failed.");
    }
    else
    {
        Console.WriteLine("Invalid ID format.");
    }
}

// ----------------------- Expense Helper Methods -----------------------

static async Task CreateExpense(IExpenseService expenseService, InputProcessor inputProcessor)
{
    Console.Clear();
    Console.WriteLine("=== Create Expense ===");

    string name = inputProcessor.GetInput("Enter expense name: ");
    decimal amount = inputProcessor.GetValidDecimal("Enter amount: ");
    DateTime date = inputProcessor.GetValidDate("Enter expense date (yyyy-mm-dd): ");
    string category = inputProcessor.GetInput("Enter category (if 'Savings', this expense will update your saving goal progress): ");

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

static async Task UpdateExpense(IExpenseService expenseService, InputProcessor inputProcessor)
{
    Console.Clear();
    Console.WriteLine("=== Update Expense ===");
    Console.Write("Enter expense ID to update: ");
    if (Guid.TryParse(Console.ReadLine(), out var id))
    {
        string name = inputProcessor.GetInput("Enter updated expense name: ");
        decimal amount = inputProcessor.GetValidDecimal("Enter updated expense amount: ");
        DateTime date = inputProcessor.GetValidDate("Enter updated expense date (yyyy-mm-dd): ");
        string category = inputProcessor.GetInput("Enter updated expense category: ");

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

    string name = inputProcessor.GetInput("Enter budget name: ");
    var frequency = inputProcessor.GetEnum("Enter frequency (Weekly/Monthly/Yearly): ", BudgetTracker.Domain.Entities.BudgetFrequency.Monthly);
    DateTime startDate = inputProcessor.GetValidDate("Enter start date (yyyy-mm-dd): ");
    DateTime endDate = inputProcessor.GetValidDate("Enter end date (yyyy-mm-dd): ");
    bool autoRenew = inputProcessor.GetBool("Auto renew? (y/n): ");

    // Collect budget items
    int itemCount = inputProcessor.GetValidInt("Enter number of budget items: ");
    var budgetItems = new List<BudgetTracker.Application.DTOs.Commands.CreateBudgetItemCommand>();

    for (int i = 0; i < itemCount; i++)
    {
        Console.WriteLine($"--- Budget Item {i + 1} ---");
        string category = inputProcessor.GetInput("Enter budgetItem category: ");
        decimal plannedAmount = inputProcessor.GetValidDecimal("Enter planned/budgeted amount: ");

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
        string name = inputProcessor.GetInput("Enter updated budget name: ");
        var frequency = inputProcessor.GetEnum("Enter updated frequency (Weekly/Monthly/Yearly): ", BudgetTracker.Domain.Entities.BudgetFrequency.Monthly);
        DateTime startDate = inputProcessor.GetValidDate("Enter updated start date (yyyy-mm-dd): ");
        DateTime endDate = inputProcessor.GetValidDate("Enter updated end date (yyyy-mm-dd): ");
        bool autoRenew = inputProcessor.GetBool("Auto renew? (y/n): ");

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

    string goalName = inputProcessor.GetInput("Enter saving goal name: ");
    decimal targetAmount = inputProcessor.GetValidDecimal("Enter target amount: ");
    decimal currentAmount = inputProcessor.GetValidDecimal("Enter current amount: ");
    DateTime? targetDate = inputProcessor.GetValidDate("Enter target date (yyyy-mm-dd): ");

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
        string goalName = inputProcessor.GetInput("Enter updated saving goal name: ");
        decimal targetAmount = inputProcessor.GetValidDecimal("Enter updated target amount: ");
        decimal currentAmount = inputProcessor.GetValidDecimal("Enter updated current amount: ");
        DateTime? targetDate = inputProcessor.GetValidDate("Enter updated target date (yyyy-mm-dd): ");

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
