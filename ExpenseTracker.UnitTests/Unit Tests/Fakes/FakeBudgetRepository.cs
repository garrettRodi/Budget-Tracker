using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BudgetTracker.Tests.UnitTests.Fakes
{
    public class FakeBudgetRepository : IBudgetRepository
    {
        private readonly List<BudgetContainer> _budgets = new List<BudgetContainer>();

        public Task AddAsync(BudgetContainer budget)
        {
            _budgets.Add(budget);
            return Task.CompletedTask;
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var budget = _budgets.FirstOrDefault(b => b.Id == id);
            if (budget != null)
            {
                _budgets.Remove(budget);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<IEnumerable<BudgetContainer>> FindAsync(Expression<Func<BudgetContainer, bool>> predicate)
        {
            var result = _budgets.AsQueryable().Where(predicate).ToList();
            return Task.FromResult<IEnumerable<BudgetContainer>>(result);
        }

        public Task<IEnumerable<BudgetContainer>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<BudgetContainer>>(_budgets);
        }

        public Task<BudgetContainer?> GetByIdAsync(Guid id)
        {
            var budget = _budgets.FirstOrDefault(b => b.Id == id);
            return Task.FromResult(budget);
        }

        public Task<bool> UpdateAsync(BudgetContainer budget)
        {
            var index = _budgets.FindIndex(b => b.Id == budget.Id);
            if (index >= 0)
            {
                _budgets[index] = budget;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
