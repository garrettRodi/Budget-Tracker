using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;

namespace BudgetTracker.Application.Interfaces
{
    public interface ISavingGoalsService
    {
        Task<SavingGoalDTO> CreateSavingGoalAsync(CreateSavingGoalCommand command);
        Task<IEnumerable<SavingGoalDTO>> GetSavingGoalsByBudgetContainerIdAsync(Guid budgetContainerId);
        Task<IEnumerable<SavingGoalDTO>> GetAllSavingGoalsAsync();
        Task<SavingGoalDTO?> GetSavingGoalByIdAsync(Guid id);
        Task<bool> UpdateSavingGoalAsync(UpdateSavingGoalCommand command);
        Task<bool> DeleteSavingGoalAsync(Guid id);
        Task RecalculateCurrentAmountAsync(Guid savingGoalId);
    }
}
