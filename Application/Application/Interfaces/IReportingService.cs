using BudgetTracker.Application.DTOs;
using Microsoft.VisualBasic;
using System;
using System.Threading.Tasks;

namespace BudgetTracker.Application.Interfaces
{
    public interface IReportingService
    {
        Task<ExpenseReportDTO> GenerateExpenseReportAsync(DateTime startDate, DateTime endDate);
        Task<BudgetReportDTO> GenerateBudgetReportAsync();
        Task<IncomeReportDTO> GenerateIncomeReportAsync(DateTime startDate, DateTime endDate);
        Task<BudgetRuleReportDTO> GenerateBudgetRuleReportAsync(string rule, DateTime startDate, DateTime endDate);
    }
}
