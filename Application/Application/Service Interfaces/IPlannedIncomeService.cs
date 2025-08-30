
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.DTOs;

namespace BudgetTracker.Application.Interfaces
{
    public interface IPlannedIncomeService
    {
        Task<PlannedIncomeDTO> CreatePlannedIncomeAsync(CreatePlannedIncomeCommand command);
        Task<IEnumerable<PlannedIncomeDTO>> GetPlannedIncomesByBudgetAsync(Guid budgetContainerId);
        Task<bool> UpdatePlannedIncomeAsync(UpdatePlannedIncomeCommand command);
        Task<bool> DeletePlannedIncomeAsync(Guid id);
    }
}
