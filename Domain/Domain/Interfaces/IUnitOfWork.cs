using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IExpenseRepository ExpenseRepository { get; }
        IIncomeRepository IncomeRepository { get; }
        IBudgetRepository BudgetRepository { get; }
        ISavingGoalsRepository SavingGoalsRepository { get; }
        ICategoryMappingRepository CategoryMappingRepository { get; }
        IPlannedExpenseRepository PlannedExpenseRepository { get; }
        IPlannedIncomeRepository PlannedIncomeRepository { get; }
        Task<int> CommitAsync();
    }
}
