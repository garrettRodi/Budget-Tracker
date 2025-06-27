using System.Linq;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.Helpers;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Domain.ValueObjects;
using BudgetTrackers.Application.DTOs;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using BudgetTracker.Application.Services;

namespace BudgetTracker.Application.Services
{
    public class ReportingService : IReportingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryMappingService _categoryMappingService;
        private readonly ILogger<ReportingService> _logger;
        private readonly ICurrencyService _currencyService;

        public ReportingService(IUnitOfWork unitOfWork, ICategoryMappingService categoryMappingService, ILogger<ReportingService> logger, ICurrencyService currencyService)
        {
            _unitOfWork = unitOfWork;
            _categoryMappingService = categoryMappingService;
            _logger = logger;
            _currencyService = currencyService;
        }

        public async Task<ExpenseReportDTO> GenerateExpenseReportAsync(Guid budgetContainerId, DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Generating expense report for budget {BudgetId} from {StartDate} to {EndDate}", budgetContainerId, startDate, endDate);

            // Filter expenses for the given budget container.
            var expenses = await _unitOfWork.ExpenseRepository.FindAsync(e =>
                 e.BudgetContainerId == budgetContainerId &&
                 e.ExpenseDate >= startDate && e.ExpenseDate <= endDate);

            Money totalExpenses = expenses.Aggregate(new Money(0,_currencyService.CurrentCurrency), (sum, e) => sum + e.Amount);

            var categoryTotals = expenses
     .GroupBy(e => e.Category)
     .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount.Amount));

            var categoryPercentages = categoryTotals.ToDictionary(
                 ct => ct.Key,
                 ct => totalExpenses.Amount > 0 ? (ct.Value / totalExpenses.Amount) * 100 : 0);

            _logger.LogInformation("Expense report generated successfully with total expenses: {TotalExpenses}", totalExpenses);

            return new ExpenseReportDTO
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalExpenses = totalExpenses,
                CategoryTotals = categoryTotals,
                CategoryPercentages = categoryPercentages
            };
        }


        public async Task<BudgetReportDTO> GenerateBudgetReportAsync(Guid budgetContainerId)
        {
            _logger.LogInformation("Generating budget report for budget container {BudgetContainerId}", budgetContainerId);

            // Retrieve the specific budget container by ID.
            var budget = await _unitOfWork.BudgetRepository.GetByIdAsync(budgetContainerId);
            if (budget == null)
            {
                _logger.LogWarning("No budget container found with ID {BudgetContainerId}", budgetContainerId);
                return new BudgetReportDTO { BudgetedExpenses = new Money(0m, _currencyService.CurrentCurrency), ActualExpenses = new Money(0m, _currencyService.CurrentCurrency), Difference = new Money(0m, _currencyService.CurrentCurrency) };
            }

            // Calculate the planned budget from the budget items.
            Money plannedBudget = budget.BudgetItems.Aggregate(new Money(0m, _currencyService.CurrentCurrency), (sum, i) => sum + i.PlannedAmount);

            // Query expenses for this container using the container’s period.
            var expenses = await _unitOfWork.ExpenseRepository.FindAsync(e =>
                e.BudgetContainerId == budgetContainerId &&
                e.ExpenseDate >= budget.StartDate &&
                e.ExpenseDate <= budget.EndDate);

            Money actualExpenses = expenses.Aggregate(new Money(0m, _currencyService.CurrentCurrency), (sum, e) => sum + e.Amount);

            _logger.LogInformation("Budget report for container {BudgetContainerId}: Planned={PlannedBudget}, Actual={ActualExpenses}",
                                     budgetContainerId, plannedBudget, actualExpenses);

            return new BudgetReportDTO
            {
                BudgetedExpenses = plannedBudget,
                ActualExpenses = actualExpenses,
                Difference = plannedBudget - actualExpenses
            };
        }


        public async Task<IncomeReportDTO> GenerateIncomeReportAsync(Guid budgetContainerId, DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Generating income report for budget {BudgetId} from {StartDate} to {EndDate}", budgetContainerId, startDate, endDate);

            // Filter incomes by both date range and BudgetContainerId.
            var incomes = await _unitOfWork.IncomeRepository.FindAsync(i =>
                i.BudgetContainerId == budgetContainerId &&
                i.ReceivedDate >= startDate && i.ReceivedDate <= endDate);

            Money totalIncome = incomes.Aggregate(new Money(0m, _currencyService.CurrentCurrency), (sum, i) => sum + i.ActualAmount);

            _logger.LogInformation("Income report generated successfully for budget {BudgetId} with total income: {TotalIncome}", budgetContainerId, totalIncome);

            return new IncomeReportDTO
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalIncome = totalIncome
            };
        }

        public async Task<BudgetRuleReportDTO> GenerateBudgetRuleReportAsync(string rule, Guid budgetContainerId, DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Generating budget rule report for rule: {Rule}, Budget: {BudgetId}, from {StartDate} to {EndDate}", rule, budgetContainerId, startDate, endDate);

            // Retrieve the budget container using its ID.
            var budgetContainer = await _unitOfWork.BudgetRepository.GetByIdAsync(budgetContainerId);
            if (budgetContainer == null)
            {
                _logger.LogWarning("No budget container found with ID {BudgetId}", budgetContainerId);
                return new BudgetRuleReportDTO { Rule = rule };
            }

            // Assume budget items are stored in budgetContainer.Items.
            // Calculate planned amounts for each category.
            Money necessitiesPlanned = budgetContainer.BudgetItems
                .Where(i => i.Category.Equals("Necessities", StringComparison.OrdinalIgnoreCase))
                .Aggregate(new Money(0, _currencyService.CurrentCurrency), (sum, i) => sum + i.PlannedAmount);
            Money savingsPlanned = budgetContainer.BudgetItems
                .Where(i => i.Category.Equals("Savings", StringComparison.OrdinalIgnoreCase))
                .Aggregate(new Money(0, _currencyService.CurrentCurrency), (sum, i) => sum + i.PlannedAmount);
            Money discretionaryPlanned = budgetContainer.BudgetItems
                .Where(i => i.Category.Equals("Discretionary", StringComparison.OrdinalIgnoreCase))
                .Aggregate(new Money(0, _currencyService.CurrentCurrency), (sum, i) => sum + i.PlannedAmount);

            // Query expenses for this container within the specified date range.
            var expenses = await _unitOfWork.ExpenseRepository.FindAsync(e =>
                e.BudgetContainerId == budgetContainerId &&
                e.ExpenseDate >= startDate && e.ExpenseDate <= endDate);
            Money necessitiesActual = expenses
                .Where(e => e.Category.Equals("Necessities", StringComparison.OrdinalIgnoreCase))
                .Aggregate(new Money(0, _currencyService.CurrentCurrency), (sum, e) => sum + e.Amount);
            Money savingsActual = expenses
                .Where(e => e.Category.Equals("Savings", StringComparison.OrdinalIgnoreCase))
                .Aggregate(new Money(0, _currencyService.CurrentCurrency), (sum, e) => sum + e.Amount);
            Money discretionaryActual = expenses
                .Where(e => e.Category.Equals("Discretionary", StringComparison.OrdinalIgnoreCase))
                .Aggregate(new Money(0, _currencyService.CurrentCurrency), (sum, e) => sum + e.Amount);

            decimal necessitiesPercentageVariance = necessitiesPlanned.Amount > 0 ? ((necessitiesPlanned.Amount - necessitiesActual.Amount) / necessitiesPlanned.Amount) * 100 : 0;
            decimal savingsPercentageVariance = savingsPlanned.Amount > 0 ? ((savingsPlanned.Amount - savingsActual.Amount) / savingsPlanned.Amount) * 100 : 0;
            decimal discretionaryPercentageVariance = discretionaryPlanned.Amount > 0 ? ((discretionaryPlanned.Amount - discretionaryActual.Amount) / discretionaryPlanned.Amount) * 100 : 0;

            _logger.LogInformation("Budget rule report generated successfully for rule: {Rule}", rule);
            return new BudgetRuleReportDTO
            {
                Rule = rule,
                NecessitiesPlanned = necessitiesPlanned,
                NecessitiesActual = necessitiesActual,
                SavingsPlanned = savingsPlanned,
                SavingsActual = savingsActual,
                DiscretionaryPlanned = discretionaryPlanned,
                DiscretionaryActual = discretionaryActual,
                NecessitiesPercentageVariance = necessitiesPercentageVariance,
                SavingsPercentageVariance = savingsPercentageVariance,
                DiscretionaryPercentageVariance = discretionaryPercentageVariance

            };
        }

        public async Task<IEnumerable<SavingGoalReportDTO>> GenerateSavingGoalReportAsync(Guid budgetContainerId)
        {
            // 1. Get all saving goals for the budget
            var savingGoals = (await _unitOfWork.SavingGoalsRepository.GetAllAsync())
                .Where(g => g.BudgetContainerId == budgetContainerId)
                .ToList();

            // 2. Get all expenses for the budget, to sum up savings
            var expenses = (await _unitOfWork.ExpenseRepository.GetAllAsync())
                .Where(e => e.BudgetContainerId == budgetContainerId)
                .ToList();

            var report = new List<SavingGoalReportDTO>();

            // 3. For each saving goal, sum expenses linked to that goal
            foreach (var goal in savingGoals)
            {
                Money currentAmount = expenses
    .Where(e => e.Category == "Savings" && e.SavingGoalId == goal.Id)
    .Aggregate(new Money(0, _currencyService.CurrentCurrency), (sum, e) => sum + e.Amount);

                report.Add(new SavingGoalReportDTO
                {
                    Id = goal.Id,
                    GoalName = goal.GoalName,
                    TargetAmount = goal.TargetAmount,
                    CurrentAmount = currentAmount,
                    TargetDate = goal.TargetDate
                });
            }

            // === CHANGE: Calculate Bulk/Uncategorized savings ===
            Money bulkSavings = expenses
                .Where(e => e.Category == "Savings" &&
                           (e.SavingGoalId == null || e.SavingGoalId == Guid.Empty))
                .Aggregate(new Money(0, _currencyService.CurrentCurrency), (sum, e) => sum + e.Amount);

            // If there are any bulk savings, add a "virtual" saving goal to the report
            if (bulkSavings.Amount > 0)
            {
                report.Add(new SavingGoalReportDTO
                {
                    Id = Guid.Empty, // or a special value to indicate "bulk"
                    GoalName = "Bulk/Uncategorized Savings",
                    TargetAmount = new Money(0m, _currencyService.CurrentCurrency), // No target for bulk savings
                    CurrentAmount = bulkSavings,
                    TargetDate = null // Or DateTime.MinValue, as appropriate
                });
            }

            return report;
        }

        public async Task<ExpenseReportDTO> GetFilteredExpensesAsync(Guid budgetContainerId, string category, DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Drill-down: filtering expenses for budget {BudgetContainerId}, category {Category}, between {StartDate} and {EndDate}",
                budgetContainerId, category, startDate, endDate);

            // Convert 'category' to lower case once.
            string lowerCategory = category.ToLower();

            // Use EF Core–friendly predicate that compares lower-case values.
            var expenses = await _unitOfWork.ExpenseRepository.FindAsync(e =>
                 e.BudgetContainerId == budgetContainerId &&
                 e.Category.ToLower() == lowerCategory &&
                 e.ExpenseDate >= startDate &&
                 e.ExpenseDate <= endDate);

            Money totalExpenses = expenses.Aggregate(new Money(0m, _currencyService.CurrentCurrency), (sum, e) => sum + e.Amount);
            var categoryTotals = expenses.GroupBy(e => e.Category)
                                         .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount.Amount));
            var categoryPercentages = categoryTotals.ToDictionary(
                ct => ct.Key,
                ct => totalExpenses.Amount > 0 ? (ct.Value / totalExpenses.Amount) * 100 : 0);

            return new ExpenseReportDTO
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalExpenses = totalExpenses,
                CategoryTotals = categoryTotals,
                CategoryPercentages = categoryPercentages
            };
        }

        public async Task<BudgetMatrixReportDTO> GenerateBudgetMatrixReportAsync(Guid budgetContainerId)
        {
            _logger.LogInformation("Generating budget matrix report for Budget {BudgetId}", budgetContainerId);

            var budget = await _unitOfWork.BudgetRepository.GetByIdAsync(budgetContainerId)
                         ?? throw new ApplicationException($"Budget {budgetContainerId} not found.");

            var start = budget.StartDate.Date;
            var end = budget.EndDate.Date;

            // 1) build the list of column‐periods
            List<DateTime> periods = budget.Frequency switch
            {
                BudgetFrequency.Weekly => Enumerable
                    .Range(0, 7)
                    .Select(d => start.AddDays(d))
                    .ToList(),

                BudgetFrequency.Monthly => Enumerable
                    .Range(0, (end - start).Days + 1)
                    .Select(d => start.AddDays(d))
                    .ToList(),

                BudgetFrequency.Yearly => Enumerable
                    .Range(0, ((end.Year - start.Year) * 12 + end.Month - start.Month) + 1)
                    .Select(m => new DateTime(start.Year, start.Month, 1).AddMonths(m))
                    .ToList(),

                _ => throw new NotSupportedException($"Frequency {budget.Frequency} not supported.")
            };

            // 2) init DTO
            var dto = new BudgetMatrixReportDTO
            {
                StartDate = start,
                EndDate = end,
                ReportingPeriods = periods,
                Categories = budget.BudgetItems
                    .Select(i => i.Category)
                    .Distinct()
                    .ToList()
            };

            // 3) zero‐fill every cell
            foreach (var cat in dto.Categories)
                foreach (var p in periods)
                {
                    dto.PlannedByCategoryAndDate[(cat, p)] = new Money (0m, _currencyService.CurrentCurrency);
                    dto.ActualByCategoryAndDate[(cat, p)] = new Money (0m, _currencyService.CurrentCurrency);
                }
            // 4) tally planned expenses from your existing PlannedExpenseRepository
            var plannedExpenses = await _unitOfWork.PlannedExpenseRepository.FindAsync(pe =>
                pe.BudgetContainerId == budgetContainerId &&
                pe.Period >= start &&
                pe.Period <= end);

            // --- NEW: ensure any planned-expense categories are in the dictionary ---
            foreach (var newCat in plannedExpenses.Select(pe => pe.Category).Distinct())
            {
                if (!dto.Categories.Contains(newCat))
                {
                    dto.Categories.Add(newCat);
                    foreach (var p in periods)
                    {
                        dto.PlannedByCategoryAndDate[(newCat, p)] = new Money(0m, _currencyService.CurrentCurrency);
                        dto.ActualByCategoryAndDate[(newCat, p)] = new Money(0m, _currencyService.CurrentCurrency);
                    }
                }
            }

            foreach (var pe in plannedExpenses)
            {
                // match the period key exactly like you do for actuals:
                var key = budget.Frequency == BudgetFrequency.Yearly
                    ? new DateTime(pe.Period.Year, pe.Period.Month, 1)
                    : pe.Period.Date;
            
                // accumulate each planned expense into its own period/category cell
                dto.PlannedByCategoryAndDate[(pe.Category, key)] += pe.Amount;
            }
            // 5) tally actuals
            var expenses = await _unitOfWork.ExpenseRepository.FindAsync(e =>
                e.BudgetContainerId == budgetContainerId
             && e.ExpenseDate >= start
             && e.ExpenseDate <= end);

            foreach (var exp in expenses)
            {
                var key = budget.Frequency == BudgetFrequency.Yearly
                    ? new DateTime(exp.ExpenseDate.Year, exp.ExpenseDate.Month, 1)
                    : exp.ExpenseDate.Date;

                var normCat = exp.Category;

                // If this category wasn't in planned, add it to categories and zero-fill
                if (!dto.Categories.Contains(normCat))
                {
                    dto.Categories.Add(normCat); // So it shows up in the matrix output

                    foreach (var p in periods)
                    {
                        dto.PlannedByCategoryAndDate[(normCat, p)] = new Money(0m, _currencyService.CurrentCurrency);
                        dto.ActualByCategoryAndDate[(normCat, p)] = new Money(0m, _currencyService.CurrentCurrency);
                    }
                }
                dto.ActualByCategoryAndDate[(normCat, key)] += exp.Amount;
            }

            // 6) tally actual income
            var incomes = await _unitOfWork.IncomeRepository.FindAsync(i =>
                i.BudgetContainerId == budgetContainerId &&
                i.ReceivedDate >= start &&
                i.ReceivedDate <= end);

            foreach (var inc in incomes)
            {
                var key = budget.Frequency == BudgetFrequency.Yearly
                    ? new DateTime(inc.ReceivedDate.Year, inc.ReceivedDate.Month, 1)
                    : inc.ReceivedDate.Date;

                // ensure the DTO has an entry for Income on that period
                if (!dto.ActualByCategoryAndDate.ContainsKey(("Income", key)))
                    dto.ActualByCategoryAndDate[("Income", key)] = new Money(0m, _currencyService.CurrentCurrency);

                dto.ActualByCategoryAndDate[("Income", key)] += inc.ActualAmount;
            }

            // 6b) tally planned income
            var plannedIncomes = await _unitOfWork.PlannedIncomeRepository.FindAsync(pi =>
                pi.BudgetContainerId == budgetContainerId &&
                pi.PeriodStart >= start &&
                pi.PeriodStart <= end);

            foreach (var pi in plannedIncomes)
            {
                var key = budget.Frequency == BudgetFrequency.Yearly
                    ? new DateTime(pi.PeriodStart.Year, pi.PeriodStart.Month, 1)
                    : pi.PeriodStart.Date;

                if (!dto.PlannedByCategoryAndDate.ContainsKey(("Income", key)))
                    dto.PlannedByCategoryAndDate[("Income", key)] = new Money(0m, _currencyService.CurrentCurrency);

                dto.PlannedByCategoryAndDate[("Income", key)] += pi.Amount;
            }


            _logger.LogInformation(
                "Budget matrix report ready: {Cats} categories over {Cols} columns",
                dto.Categories.Count, periods.Count);

            return dto;
        }
    }
}
