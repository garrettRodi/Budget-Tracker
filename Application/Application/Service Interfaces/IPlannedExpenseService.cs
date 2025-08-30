using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;

namespace BudgetTracker.Application.Interfaces
{
    public interface IPlannedExpenseService
    {
        Task<PlannedExpenseDTO> CreatePlannedExpenseAsync(CreatePlannedExpenseCommand cmd);
        Task<IEnumerable<PlannedExpenseDTO>> ViewPlannedExpensesAsync(Guid budgetContainerId);
        Task<bool> UpdatePlannedExpenseAsync(UpdatePlannedExpenseCommand cmd);
        Task<bool> DeletePlannedExpenseAsync(Guid id);
    }
}
