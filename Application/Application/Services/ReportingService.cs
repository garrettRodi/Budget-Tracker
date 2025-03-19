using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetTracker.Application.Services
{
    public class ReportingService : IReportingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryMappingService _categoryMappingService;

        public ReportingService(IUnitOfWork unitOfWork, ICategoryMappingService categoryMappingService)
        {
            _unitOfWork = unitOfWork;
            _categoryMappingService = categoryMappingService;
        }

        // Generates an enhanced expense report that includes total expenses,
        // breakdown per category, and the percentage each category represents.
        public async Task<ExpenseReportDTO> GenerateExpenseReportAsync(DateTime startDate, DateTime endDate)
        {
            var expenses = await _unitOfWork.ExpenseRepository.FindAsync(e => e.ExpenseDate >= startDate && e.ExpenseDate <= endDate);
            decimal totalExpenses = expenses.Sum(e => e.Amount);

            var categoryTotals = expenses.GroupBy(e => e.Category)
                                         .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

            var categoryPercentages = categoryTotals.ToDictionary(ct => ct.Key,
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

        // Generates a report comparing the planned budget with the actual expenses for the latest budget period.
        public async Task<BudgetReportDTO> GenerateBudgetReportAsync()
        {
            var budgets = await _unitOfWork.BudgetRepository.GetAllAsync();
            var latestBudget = budgets.OrderByDescending(b => b.StartDate).FirstOrDefault();
            if (latestBudget == null)
            {
                return new BudgetReportDTO { BudgetedExpenses = 0, ActualExpenses = 0, Difference = 0 };
            }

            decimal plannedBudget = latestBudget.Items.Sum(i => i.PlannedAmount);
            var expenses = await _unitOfWork.ExpenseRepository.FindAsync(e =>
                e.ExpenseDate >= latestBudget.StartDate && e.ExpenseDate <= latestBudget.EndDate);
            decimal actualExpenses = expenses.Sum(e => e.Amount);

            return new BudgetReportDTO
            {
                BudgetedExpenses = plannedBudget,
                ActualExpenses = actualExpenses,
                Difference = plannedBudget - actualExpenses
            };
        }

        // Generates an income report over a specified period.
        public async Task<IncomeReportDTO> GenerateIncomeReportAsync(DateTime startDate, DateTime endDate)
        {
            var incomes = await _unitOfWork.IncomeRepository.FindAsync(i => i.ReceivedDate >= startDate && i.ReceivedDate <= endDate);
            decimal totalIncome = incomes.Sum(i => i.Amount);

            return new IncomeReportDTO
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalIncome = totalIncome
            };
        }

        // Generates a report that compares the planned and actual amounts for a given budget rule,
        // such as "50/20/30", by calculating variances for Necessities, Savings, and Discretionary spending.
        public async Task<BudgetRuleReportDTO> GenerateBudgetRuleReportAsync(string rule, DateTime startDate, DateTime endDate)
        {
            // Assume that expenses and budget items are categorized as "Necessities", "Savings", or "Discretionary".
            var budgets = await _unitOfWork.BudgetRepository.GetAllAsync();
            var latestBudget = budgets.OrderByDescending(b => b.StartDate).FirstOrDefault();
            if (latestBudget == null)
            {
                return new BudgetRuleReportDTO { Rule = rule };
            }

            // Load all mappings once for efficiency.
            var mappings = await _categoryMappingService.GetAllMappingsAsync();
            var mappingDictionary = mappings.ToDictionary(m => m.CategoryName, m => m.GroupName, StringComparer.OrdinalIgnoreCase);

            // Planned amounts from the latest budget.
            decimal necessitiesPlanned = latestBudget.Items
                .Where(i => i.Category.Equals("Necessities", StringComparison.OrdinalIgnoreCase))
                .Sum(i => i.PlannedAmount);
            decimal savingsPlanned = latestBudget.Items.Where(i => i.Category.Equals("Savings", StringComparison.OrdinalIgnoreCase))
                .Sum(i => i.PlannedAmount);
            decimal discretionaryPlanned = latestBudget.Items
                .Where(i => i.Category.Equals("Discretionary", StringComparison.OrdinalIgnoreCase)).Sum(i => i.PlannedAmount);

            // Actual expenses in the budget period.
            var expenses = await _unitOfWork.ExpenseRepository.FindAsync(e => e.ExpenseDate >= latestBudget.StartDate && e.ExpenseDate <= latestBudget.EndDate);
            decimal necessitiesActual = expenses
                .Where(e => e.Category.Equals("Necessities", StringComparison.OrdinalIgnoreCase))
                .Sum(e => e.Amount);
            decimal savingsActual = expenses
                .Where(e => e.Category.Equals("Savings", StringComparison.OrdinalIgnoreCase))
                .Sum(e => e.Amount);
            decimal discretionaryActual = expenses
                .Where(e => e.Category.Equals("Discretionary", StringComparison.OrdinalIgnoreCase))
                .Sum(e => e.Amount);

            // Calculate percentage variances.
            decimal necessitiesPercentageVariance = necessitiesPlanned > 0 ? ((necessitiesPlanned - necessitiesActual) / necessitiesPlanned) * 100 : 0;
            decimal savingsPercentageVariance = savingsPlanned > 0 ? ((savingsPlanned - savingsActual) / savingsPlanned) * 100 : 0;
            decimal discretionaryPercentageVariance = discretionaryPlanned > 0 ? ((discretionaryPlanned - discretionaryActual) / discretionaryPlanned) * 100 : 0;

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
    }
}
