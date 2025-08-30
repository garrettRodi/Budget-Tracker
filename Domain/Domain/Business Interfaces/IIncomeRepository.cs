using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Domain.Interfaces
{
    public interface IIncomeRepository : IGenericRepository<Income>
    {
        public Task AddAsync(Income income);
        Task<Income?> GetByIdAsync(Guid id);
        Task<IEnumerable<Income>> GetAllAsync();
        Task<bool> UpdateAsync(Income income);
        Task<bool> DeleteAsync(Guid id);

    }
}
