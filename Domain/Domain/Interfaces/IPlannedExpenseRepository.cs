using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Domain.Interfaces
{
    public interface IPlannedExpenseRepository : IGenericRepository<PlannedExpense>
    {
        Task<IEnumerable<PlannedExpense>> GetPlannedExpensesByBudgetAsync(Guid budgetContainerId);
    }
}
