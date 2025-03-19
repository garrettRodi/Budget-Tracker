using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;

namespace BudgetTracker.Application.Interfaces
{
    public interface IBudgetService
    {
        Task<BudgetDTO> CreateBudgetAsync(CreateBudgetCommand command);
        Task<BudgetDTO?> GetBudgetByIdAsync(Guid id);
        Task<IEnumerable<BudgetDTO>> GetAllBudgetsAsync();
        Task<bool> UpdateBudgetAsync(UpdateBudgetCommand command);
        Task<bool> DeleteBudgetAsync(Guid id);
    }
}
