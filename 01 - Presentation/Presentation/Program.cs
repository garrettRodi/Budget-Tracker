using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using BudgetTracker.Infrastructure.DataAccess;
using BudgetTracker.Infrastructure.RepositoryImplementations;
using BudgetTracker.Application.Services;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;
using BudgetTracker.Infrastructure.ExternalServices;
using BudgetTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Presentation.PresentationHelpers;
using BudgetTracker.Presentation.ReportingHelpers;

namespace BudgetTracker.Presentation
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // 1) Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()           // ← write to console as well
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting up...");

                // 2) Build Host
                var host = Host.CreateDefaultBuilder(args)
                    .UseSerilog()
                    .ConfigureServices((context, services) =>
                    {
                        // Infrastructure / EF Core
                        var dbFile = Path.Combine("Database", "BudgetTracker.db");
                        Directory.CreateDirectory("Database");
                        services.AddDbContext<BudgetTrackerDbContext>(opts =>
                            opts.UseSqlite(@"Data Source=BudgetTracker.db",
                                b => b.MigrationsAssembly("04 - Infrastructure")));

                        //Console abstraction as singleton
                        services.AddSingleton<IConsole, SystemConsole>();

                        //UI Helpers (depend on IConsole)
                        services.AddScoped<InputProcessor>();
                        services.AddScoped<MainMenu>();
                        services.AddScoped<IncomeMenu>();
                        services.AddScoped<ExpenseMenu>();
                        services.AddScoped<BudgetMenu>();
                        services.AddScoped<ReportingMenu>();
                        services.AddScoped<SavingGoalsMenu>();
                        services.AddScoped<SelectBudgetContainer>();

                        // Presentation-level helpers
                        services.AddScoped<ExpenseHelpers>();
                        services.AddScoped<PlannedExpenseHelpers>();
                        services.AddScoped<IncomeHelpers>();
                        services.AddScoped<PlannedIncomeHelpers>();
                        services.AddScoped<BudgetHelpers>();
                        services.AddScoped<SavingGoalsHelpers>();

                        // Presentation-level reporting helpers
                        services.AddScoped<BudgetReportingHelpers>();
                        services.AddScoped<DrillDownReport>();
                        services.AddScoped<ExpenseReportHelpers>();
                        services.AddScoped<IncomeReportingHelpers>();
                        services.AddScoped<ReportDashboard>();
                        services.AddScoped<SavingGoalsReportingHelpers>();

                        
                        // Application services and repositories
                        services.AddScoped<IUnitOfWork, UnitOfWork>();
                        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
                        services.AddScoped<ExpenseRepository>();
                        services.AddScoped<IExpenseService, ExpenseService>();
                        services.AddScoped<BudgetRepository>();
                        services.AddScoped<IBudgetService, BudgetService>();
                        services.AddScoped<IncomeRepository>();
                        services.AddScoped<IIncomeService, IncomeService>();
                        services.AddScoped<SavingGoalsRepository>();
                        services.AddScoped<ISavingGoalsService, SavingGoalsService>();
                        services.AddScoped<CategoryMappingRepository>();
                        services.AddScoped<ICategoryMappingService, CategoryMappingService>();
                        services.AddScoped<IReportingService, ReportingService>();
                        services.AddScoped<IPlannedIncomeRepository, PlannedIncomeRepository>();
                        services.AddScoped<IPlannedIncomeService, PlannedIncomeService>();
                        services.AddScoped<IPlannedExpenseRepository, PlannedExpenseRepository>();
                        services.AddScoped<IPlannedExpenseService, PlannedExpenseService>();

                        // Currency conversion
                        services.AddHttpClient<CurrencyConversionService>();

                        // App Controller
                        services.AddScoped<AppController>();
                    })
                    .Build();

                // 3) Ensure DB is created/migrated
                Console.WriteLine("== BEFORE MIGRATE ==");
                using (var scope = host.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<BudgetTrackerDbContext>();
                    db.Database.Migrate();
                }

                // 4) Run the app
                Console.WriteLine("== BEFORE RUNAPP ==");
                using (var scope = host.Services.CreateScope())
                {
                    var app = scope.ServiceProvider.GetRequiredService<AppController>();
                    Console.WriteLine("About to invoke RunAsync()");
                    await app.RunAsync();
                    Console.WriteLine("Returned from RunAsync()");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ UNCAUGHT STARTUP EXCEPTION: " + ex);
                Log.Fatal(ex, "Application start-up failed.");
            }
            finally
            {
                Log.CloseAndFlush();
                Console.WriteLine("== END OF MAIN ==");
                Console.ReadLine();
            }
        }
    }
}
