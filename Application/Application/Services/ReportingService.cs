using System.Linq;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.Helpers;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using BudgetTrackers.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Application.Services
{
    public class ReportingService : IReportingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryMappingService _categoryMappingService;
        private readonly ILogger<ReportingService> _logger;

        public ReportingService(IUnitOfWork unitOfWork, ICategoryMappingService categoryMappingService, ILogger<ReportingService> logger)
        {
            _unitOfWork = unitOfWork;
            _categoryMappingService = categoryMappingService;
            _logger = logger;
        }

        public async Task<ExpenseReportDTO> GenerateExpenseReportAsync(Guid budgetContainerId, DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Generating expense report for budget {BudgetId} from {StartDate} to {EndDate}", budgetContainerId, startDate, endDate);

            // Filter expenses for the given budget container.
            var expenses = await _unitOfWork.ExpenseRepository.FindAsync(e =>
                 e.BudgetContainerId == budgetContainerId &&
                 e.ExpenseDate >= startDate && e.ExpenseDate <= endDate);

            decimal totalExpenses = expenses.Sum(e => e.Amount);

            var categoryTotals = expenses
     .GroupBy(e => CategoryHelper.NormalizeCategory(e.Category))
     .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

            var categoryPercentages = categoryTotals.ToDictionary(
                 ct => ct.Key,
                 ct => totalExpenses > 0 ? (ct.Value / totalExpenses) * 100 : 0);

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
                return new BudgetReportDTO { BudgetedExpenses = 0, ActualExpenses = 0, Difference = 0 };
            }

            // Calculate the planned budget from the budget items.
            decimal plannedBudget = budget.BudgetItems.Sum(i => i.PlannedAmount);

            // Query expenses for this container using the container’s period.
            var expenses = await _unitOfWork.ExpenseRepository.FindAsync(e =>
                e.BudgetContainerId == budgetContainerId &&
                e.ExpenseDate >= budget.StartDate &&
                e.ExpenseDate <= budget.EndDate);

            decimal actualExpenses = expenses.Sum(e => e.Amount);

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

            decimal totalIncome = incomes.Sum(i => i.ActualAmount);

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
            decimal necessitiesPlanned = budgetContainer.BudgetItems
                .Where(i => i.Category.Equals("Necessities", StringComparison.OrdinalIgnoreCase))
                .Sum(i => i.PlannedAmount);
            decimal savingsPlanned = budgetContainer.BudgetItems
                .Where(i => i.Category.Equals("Savings", StringComparison.OrdinalIgnoreCase))
                .Sum(i => i.PlannedAmount);
            decimal discretionaryPlanned = budgetContainer.BudgetItems
                .Where(i => i.Category.Equals("Discretionary", StringComparison.OrdinalIgnoreCase))
                .Sum(i => i.PlannedAmount);

            // Query expenses for this container within the specified date range.
            var expenses = await _unitOfWork.ExpenseRepository.FindAsync(e =>
                e.BudgetContainerId == budgetContainerId &&
                e.ExpenseDate >= startDate && e.ExpenseDate <= endDate);
            decimal necessitiesActual = expenses
                .Where(e => e.Category.Equals("Necessities", StringComparison.OrdinalIgnoreCase))
                .Sum(e => e.Amount);
            decimal savingsActual = expenses
                .Where(e => e.Category.Equals("Savings", StringComparison.OrdinalIgnoreCase))
                .Sum(e => e.Amount);
            decimal discretionaryActual = expenses
                .Where(e => e.Category.Equals("Discretionary", StringComparison.OrdinalIgnoreCase))
                .Sum(e => e.Amount);

            decimal necessitiesPercentageVariance = necessitiesPlanned > 0 ? ((necessitiesPlanned - necessitiesActual) / necessitiesPlanned) * 100 : 0;
            decimal savingsPercentageVariance = savingsPlanned > 0 ? ((savingsPlanned - savingsActual) / savingsPlanned) * 100 : 0;
            decimal discretionaryPercentageVariance = discretionaryPlanned > 0 ? ((discretionaryPlanned - discretionaryActual) / discretionaryPlanned) * 100 : 0;

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
                decimal currentAmount = expenses
                    .Where(e => e.Category == "Savings" && e.SavingGoalId == goal.Id)
                    .Sum(e => e.Amount);

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
            decimal bulkSavings = expenses
                .Where(e => e.Category == "Savings" &&
                           (e.SavingGoalId == null || e.SavingGoalId == Guid.Empty))
                .Sum(e => e.Amount);

            // If there are any bulk savings, add a "virtual" saving goal to the report
            if (bulkSavings > 0)
            {
                report.Add(new SavingGoalReportDTO
                {
                    Id = Guid.Empty, // or a special value to indicate "bulk"
                    GoalName = "Bulk/Uncategorized Savings",
                    TargetAmount = 0, // No target for bulk savings
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

            decimal totalExpenses = expenses.Sum(e => e.Amount);
            var categoryTotals = expenses.GroupBy(e => e.Category)
                                         .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));
            var categoryPercentages = categoryTotals.ToDictionary(
                ct => ct.Key,
                ct => totalExpenses > 0 ? (ct.Value / totalExpenses) * 100 : 0);

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
                    .Select(i => CategoryHelper.NormalizeCategory(i.Category))
                    .Distinct()
                    .ToList()
            };

            // 3) zero‐fill every cell
            foreach (var cat in dto.Categories)
                foreach (var p in periods)
                {
                    dto.PlannedByCategoryAndDate[(cat, p)] = 0m;
                    dto.ActualByCategoryAndDate[(cat, p)] = 0m;
                }

            // 4) distribute planned evenly
            foreach (var item in budget.BudgetItems)
            {
                var normCat = CategoryHelper.NormalizeCategory(item.Category);
                var perPeriod = item.PlannedAmount / periods.Count;
                foreach (var p in periods)
                    dto.PlannedByCategoryAndDate[(item.Category, p)] += perPeriod;
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

                var normCat = CategoryHelper.NormalizeCategory(exp.Category);

                dto.ActualByCategoryAndDate[(exp.Category, key)] += exp.Amount;

                // Only tally if the key exists (avoid crash)
                if (dto.ActualByCategoryAndDate.ContainsKey((normCat, key)))
                {
                    dto.ActualByCategoryAndDate[(normCat, key)] += exp.Amount;
                }
                // else ignore or log warning
            }

            _logger.LogInformation(
                "Budget matrix report ready: {Cats} categories over {Cols} columns",
                dto.Categories.Count, periods.Count);

            return dto;
        }
    }
}
