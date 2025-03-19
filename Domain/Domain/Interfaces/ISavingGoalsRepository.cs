using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Domain.Interfaces
{
    public interface ISavingGoalsRepository : IGenericRepository<SavingGoals>
    {
        Task AddAsync(SavingGoals goal);
        Task<SavingGoals?> GetByIdAsync(Guid id);
        Task<IEnumerable<SavingGoals>> GetAllAsync();
        Task<bool> UpdateAsync(SavingGoals goal);
        Task<bool> DeleteAsync(Guid id);
    }
}
