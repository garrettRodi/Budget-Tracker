using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Domain.Interfaces
{
    public interface IExpenseRepository : IGenericRepository<Expense>
    {
        Task AddAsync(Expense expense);
        Task<Expense?> GetByIdAsync(Guid id);
        Task<IEnumerable<Expense>> GetAllAsync();

        Task<bool> UpdateAsync(Expense expense);
        Task<bool> DeleteAsync(Guid id);
        
    }
}
