// File: 01 - Presentation/Presentation/Program.cs
using System;
using System.IO;
using System.Linq;
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
using BudgetTracker.Application.Mappers;
using Polly;                                  // ← Added for retry policies
using Polly.Extensions.Http;                  // ← Added for HttpClient Polly extensions

namespace BudgetTracker.Presentation
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // 1) Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.WriteTo.Console()
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

                        // Console abstraction as singleton
                        services.AddSingleton<IConsole, SystemConsole>();

                        // UI Helpers
                        services.AddScoped<InputProcessor>();
                        services.AddScoped<MainMenu>();
                        services.AddScoped<IncomeMenu>();
                        services.AddScoped<ExpenseMenu>();
                        services.AddScoped<BudgetMenu>();
                        services.AddScoped<ReportingMenu>();
                        services.AddScoped<SavingGoalsMenu>();
                        services.AddScoped<SettingsMenu>();
                        services.AddScoped<SelectBudgetContainer>();

                        // Presentation‐level helpers
                        services.AddScoped<ExpenseHelpers>();
                        services.AddScoped<PlannedExpenseHelpers>();
                        services.AddScoped<IncomeHelpers>();
                        services.AddScoped<PlannedIncomeHelpers>();
                        services.AddScoped<BudgetHelpers>();
                        services.AddScoped<SavingGoalsHelpers>();

                        // Presentation‐level reporting helpers
                        services.AddScoped<BudgetReportingHelpers>();
                        services.AddScoped<DrillDownReport>();
                        services.AddScoped<ExpenseReportHelpers>();
                        services.AddScoped<IncomeReportingHelpers>();
                        services.AddScoped<ReportDashboard>();
                        services.AddScoped<SavingGoalsReportingHelpers>();

                        // Application services and repositories
                        services.AddScoped<IUnitOfWork, UnitOfWork>();
                        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
                        services.AddScoped<IExpenseService, ExpenseService>();
                        services.AddScoped<IBudgetService, BudgetService>();
                        services.AddScoped<IIncomeService, IncomeService>();
                        services.AddScoped<ISavingGoalsService, SavingGoalsService>();
                        services.AddScoped<ICategoryMappingService, CategoryMappingService>();
                        services.AddScoped<IReportingService, ReportingService>();
                        services.AddScoped<IPlannedIncomeService, PlannedIncomeService>();
                        services.AddScoped<IPlannedExpenseService, PlannedExpenseService>();

                        // Currency conversion: configure HttpClient with Polly retry
                        services
                            .AddHttpClient<ICurrencyConversionService, CurrencyConversionService>(client =>
                            {
                                // Base URL for all conversion requests
                                client.BaseAddress = new Uri("https://open.er-api.com/v6/");
                            })
                            // Retry up to 3 times on transient failures, with exponential back‐off
                            .AddTransientHttpErrorPolicy(policy =>
                                policy.WaitAndRetryAsync(
                                    retryCount: 3,
                                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
                                )
                            );

                        // Application‐level currency service
                        services.AddScoped<ICurrencyService, CurrencyService>();

                        // App Controller
                        services.AddScoped<AppController>();
                    })
                    .Build();

                // 3) Ensure DB is created/migrated
                using (var scope = host.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<BudgetTrackerDbContext>();
                    db.Database.Migrate();
                }

                // 4) Run the app
                using (var scope = host.Services.CreateScope())
                {
                    var app = scope.ServiceProvider.GetRequiredService<AppController>();
                    await app.RunAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed.");
            }
            finally
            {
                Log.CloseAndFlush();
                Console.ReadLine();
            }
        }
    }
}
