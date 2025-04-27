using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BudgetTracker.Infrastructure.DataAccess;
using BudgetTracker.Infrastructure.RepositoryImplementations;
using _04__Infrastructure.Migrations;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.ExternalServices;
using BudgetTracker.Application.Services;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Presentation.ReportingHelpers;
using BudgetTracker.Presentation.IncomeHelpers;
using BudgetTracker.Presentation.ExpenseHelpers;
using BudgetTracker.Presentation.SavingGoalsHelpers;
using BudgetTracker.Presentation.BudgetHelpers;
using BudgetTracker.Presentation.UIHelpers;
using Serilog;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

string relativeDirectory = "Database";
string dbFilePath = "BudgetTracker.db";

// Ensure the directory exists.
if (!Directory.Exists(relativeDirectory))
{
    Directory.CreateDirectory(relativeDirectory);
    Console.WriteLine("Created directory: " + relativeDirectory);
}


// COnfigures Serilog globally
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    //.WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Console.WriteLine("Starting Budget Tracker...");

    // The Host builder that sets up DI
    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog() // Using Serilog as the logging provider.
        .ConfigureServices((context, services) =>
        {
            // Use the created dbFilePath in the connection string.
            services.AddDbContext<BudgetTrackerDbContext>(options =>
                options.UseSqlite($@"Data Source={dbFilePath}",
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

            // Register UnitOfWork.
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        })
        .Build();

    // Create a DI scope and force deletion using EnsureDeleted() as well.
    using var scope = host.Services.CreateScope();
    var provider = scope.ServiceProvider;
    var dbContext = provider.GetRequiredService<BudgetTrackerDbContext>();
    
    /* bool wasDeleted = dbContext.Database.EnsureDeleted();
    Console.WriteLine("EnsureDeleted returned: " + wasDeleted); */
    dbContext.Database.Migrate();
    Console.WriteLine("Database migration complete.");

    // Resolve UI Helpers and application services.
    var menu = provider.GetRequiredService<Menu>();
    var inputProcessor = provider.GetRequiredService<InputProcessor>();
    var incomeService = provider.GetRequiredService<IIncomeService>();
    var expenseService = provider.GetRequiredService<IExpenseService>();
    var budgetService = provider.GetRequiredService<IBudgetService>();
    var savingGoalsService = provider.GetRequiredService<ISavingGoalsService>();
    var reportingService = provider.GetRequiredService<IReportingService>();


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
                    await ExpenseHelpers.CreateExpense(expenseService, inputProcessor, budgetService);
                    break;
                case "2":
                    await ExpenseHelpers.ViewExpenses(expenseService, budgetService);
                    break;
                case "3":
                    await ExpenseHelpers.UpdateExpense(expenseService, inputProcessor, budgetService);
                    break;
                case "4":
                    await ExpenseHelpers.DeleteExpense(expenseService, budgetService);
                    break;
                case "5":
                    await BudgetHelpers.CreateBudget(budgetService, inputProcessor);
                    break;
                case "6":
                    await BudgetHelpers.ViewBudgets(budgetService);
                    break;
                case "7":
                    await BudgetHelpers.UpdateBudget(budgetService, inputProcessor);
                    break;
                case "8":
                    await BudgetHelpers.DeleteBudget(budgetService, inputProcessor);
                    break;
                case "9":
                    await SavingGoalsHelpers.CreateSavingGoal(savingGoalsService, inputProcessor, budgetService);
                    break;
                case "10":
                    await SavingGoalsHelpers.ViewSavingGoals(savingGoalsService, budgetService);
                    break;
                case "11":
                    await SavingGoalsHelpers.UpdateSavingGoal(savingGoalsService, inputProcessor, budgetService);
                    break;
                case "12":
                    await SavingGoalsHelpers.DeleteSavingGoal(savingGoalsService, inputProcessor, budgetService);
                    break;
                case "13":
                    await IncomeHelpers.CreateIncome(incomeService, inputProcessor, budgetService);
                    break;
                case "14":
                    await IncomeHelpers.ViewIncomes(incomeService, budgetService);
                    break;
                case "15":
                    await IncomeHelpers.UpdateIncome(incomeService, inputProcessor, budgetService);
                    break;
                case "16":
                    await IncomeHelpers.DeleteIncome(incomeService, inputProcessor, budgetService);
                    break;
                // Reporting Options:
                case "17":
                    await IncomeReportingHelpers.ViewIncomeReport(reportingService, inputProcessor, budgetService);
                    break;
                case "18":
                    await ExpenseReportHelpers.ViewExpenseReport(reportingService, inputProcessor, budgetService);
                    break;
                case "19":
                    await SavingGoalsReportingHelpers.ViewSavingGoalsReport(reportingService, budgetService);
                    break;
                case "20":
                    await BudgetReportingHelpers.ViewBudgetRuleReport(reportingService, inputProcessor, budgetService);
                    break;  
                case "21":
                    await ReportDashboard.ViewDashboard(reportingService, inputProcessor, budgetService, expenseService);
                    break;
                case "22":
                    await DrillDown.DrillDownReport(reportingService, inputProcessor, budgetService);
                    break;
                case "23":
                    await BudgetReportingHelpers.ViewBudgetMatrixReportAsync(reportingService, inputProcessor, budgetService);
                    break;
                case "24":
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
            Log.Warning(appEx, "An application error occurred.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Exception: " + ex.GetType().Name + " – " + ex.Message);
            Console.WriteLine(ex.StackTrace);
            Log.Warning(ex, "Unexpected error in main loop");
        }

        if (!exitRequested)
        {
            Console.WriteLine("Press any key to continue...Program.cs");
            Console.ReadKey();
        }
    }
}
catch (Exception ex)
{
   Log.Fatal(ex, "Application start-up failed.");
}
finally
{
    Log.CloseAndFlush();
}