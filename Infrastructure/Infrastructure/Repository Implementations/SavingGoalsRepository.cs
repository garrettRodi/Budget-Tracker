using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Infrastructure.RepositoryImplementations
{
    public class SavingGoalsRepository : GenericRepository<SavingGoals>, ISavingGoalsRepository
    {
        private readonly BudgetTrackerDbContext _context;

        public SavingGoalsRepository(BudgetTrackerDbContext context)
            : base(context)
        { }
        

        public async Task AddAsync(SavingGoals goal)
        {
            await _context.SavingGoals.AddAsync(goal);
            await _context.SaveChangesAsync();
        }

        public async Task<SavingGoals?> GetByIdAsync(Guid id)
        {
            return await _context.SavingGoals.FindAsync(id);
        }

        public async Task<IEnumerable<SavingGoals>> GetAllAsync()
        {
            return await _context.SavingGoals.ToListAsync();
        }

        public async Task<bool> UpdateAsync(SavingGoals goal)
        {
            _context.SavingGoals.Update(goal);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var goal = await _context.SavingGoals.FindAsync(id);
            if (goal == null)
                return false;
            _context.SavingGoals.Remove(goal);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
