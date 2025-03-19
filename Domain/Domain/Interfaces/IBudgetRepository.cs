using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Domain.Interfaces
{
    public interface IBudgetRepository : IGenericRepository<BudgetContainer>
    {
        Task AddAsync(BudgetContainer budget);
        Task<BudgetContainer?> GetByIdAsync(Guid id);
        Task<IEnumerable<BudgetContainer>> GetAllAsync();
        Task<bool> UpdateAsync(BudgetContainer budget);
        Task<bool> DeleteAsync(Guid id);
    }
}
