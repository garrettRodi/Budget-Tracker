using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BudgetTracker.Tests.UnitTests.Fakes
{
    public class FakeSavingGoalsRepository : ISavingGoalsRepository
    {
        private readonly List<SavingGoals> _goals = new List<SavingGoals>();
        private readonly List<SavingGoals> _fakeSavingGoals = new();


        public Task AddAsync(SavingGoals goal)
        {
            _goals.Add(goal);
            return Task.CompletedTask;
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var goal = _goals.FirstOrDefault(g => g.Id == id);
            if (goal != null)
            {
                _goals.Remove(goal);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<IEnumerable<SavingGoals>> FindAsync(Expression<Func<SavingGoals, bool>> predicate)
        {
            var result = _goals.AsQueryable().Where(predicate).ToList();
            return Task.FromResult<IEnumerable<SavingGoals>>(result);
        }

        public Task<IEnumerable<SavingGoals>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<SavingGoals>>(_goals);
        }

        public Task<SavingGoals?> GetByIdAsync(Guid id)
        {
            var goal = _goals.FirstOrDefault(g => g.Id == id);
            return Task.FromResult(goal);
        }

        public Task<bool> UpdateAsync(SavingGoals goal)
        {
            var index = _goals.FindIndex(g => g.Id == goal.Id);
            if (index >= 0)
            {
                _goals[index] = goal;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<SavingGoals?> GetGoalWithExpensesAsync(Guid id)
        {
            var goal = _fakeSavingGoals.FirstOrDefault(g => g.Id == id);
            return Task.FromResult(goal);
        }

    }
}
