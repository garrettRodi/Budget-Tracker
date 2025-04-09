using BudgetTracker.Application.DTOs;
using Microsoft.VisualBasic;
using System;
using System.Threading.Tasks;

namespace BudgetTracker.Application.Interfaces
{
    public interface IReportingService
    {
        Task<ExpenseReportDTO> GenerateExpenseReportAsync(Guid budgetContainerId, DateTime startDate, DateTime endDate);
        Task<BudgetReportDTO> GenerateBudgetReportAsync(Guid budgetContainerId);
        Task<IncomeReportDTO> GenerateIncomeReportAsync(Guid budgetContainerId, DateTime startDate, DateTime endDate);
        Task<BudgetRuleReportDTO> GenerateBudgetRuleReportAsync(string rule, Guid budgetContainerId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<SavingGoalReportDTO>> GenerateSavingGoalReportAsync(Guid budgetContainerId);
        Task<ExpenseReportDTO> GetFilteredExpensesAsync(Guid budgetContainerId, string category, DateTime startDate, DateTime endDate);

    }
}
