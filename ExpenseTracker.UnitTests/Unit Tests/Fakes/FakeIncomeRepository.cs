using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BudgetTracker.Tests.UnitTests.Fakes
{
    public class FakeIncomeRepository : IIncomeRepository
    {
        private readonly List<Income> _incomes = new List<Income>();

        public Task AddAsync(Income income)
        {
            _incomes.Add(income);
            return Task.CompletedTask;
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var income = _incomes.FirstOrDefault(i => i.Id == id);
            if (income != null)
            {
                _incomes.Remove(income);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<IEnumerable<Income>> FindAsync(Expression<Func<Income, bool>> predicate)
        {
            var result = _incomes.AsQueryable().Where(predicate).ToList();
            return Task.FromResult<IEnumerable<Income>>(result);
        }

        public Task<IEnumerable<Income>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Income>>(_incomes);
        }

        public Task<Income?> GetByIdAsync(Guid id)
        {
            var income = _incomes.FirstOrDefault(i => i.Id == id);
            return Task.FromResult(income);
        }

        public Task<bool> UpdateAsync(Income income)
        {
            var index = _incomes.FindIndex(i => i.Id == income.Id);
            if (index >= 0)
            {
                _incomes[index] = income;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
