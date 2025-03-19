using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;

namespace BudgetTracker.Application.Interfaces
{
    public interface ISavingGoalsService
    {
        Task<SavingGoalDTO> CreateSavingGoalAsync(CreateSavingGoalCommand command);
        Task<SavingGoalDTO?> GetSavingGoalByIdAsync(Guid id);
        Task<IEnumerable<SavingGoalDTO>> GetAllSavingGoalsAsync();
        Task<bool> UpdateSavingGoalAsync(UpdateSavingGoalCommand command);
        Task<bool> DeleteSavingGoalAsync(Guid id);
    }
}
