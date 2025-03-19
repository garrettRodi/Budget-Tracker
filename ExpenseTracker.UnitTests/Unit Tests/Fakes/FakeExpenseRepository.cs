using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BudgetTracker.Tests.UnitTests.Fakes
{
    public class FakeExpenseRepository : IExpenseRepository
    {
        private readonly List<Expense> _expenses = new List<Expense>();

        public Task AddAsync(Expense expense)
        {
            _expenses.Add(expense);
            return Task.CompletedTask;
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var expense = _expenses.FirstOrDefault(e => e.Id == id);
            if (expense != null)
            {
                _expenses.Remove(expense);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<IEnumerable<Expense>> FindAsync(Expression<Func<Expense, bool>> predicate)
        {
            var result = _expenses.AsQueryable().Where(predicate).ToList();
            return Task.FromResult<IEnumerable<Expense>>(result);
        }

        public Task<IEnumerable<Expense>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Expense>>(_expenses);
        }

        public Task<Expense?> GetByIdAsync(Guid id)
        {
            var expense = _expenses.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(expense);
        }

        public Task<bool> UpdateAsync(Expense expense)
        {
            var index = _expenses.FindIndex(e => e.Id == expense.Id);
            if (index >= 0)
            {
                _expenses[index] = expense;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
