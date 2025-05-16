using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Domain.Interfaces
{
    public interface IPlannedIncomeRepository
    {
        Task AddAsync(PlannedIncome plannedIncome);
        Task<PlannedIncome?> GetByIdAsync(Guid id);
        Task<IEnumerable<PlannedIncome>> FindByBudgetAsync(Guid budgetContainerId);
        Task<bool> UpdateAsync(PlannedIncome plannedIncome);
        Task<bool> DeleteAsync(Guid id);
    }
}
