using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Infrastructure.RepositoryImplementations
{
    public class BudgetRepository : GenericRepository<BudgetContainer>, IBudgetRepository
    {
        private readonly BudgetTrackerDbContext _context;

        public BudgetRepository(BudgetTrackerDbContext context)
            : base(context)
        { }

        public async Task AddAsync(BudgetContainer budget)
        {
            await _context.BudgetContainers.AddAsync(budget);
            await _context.SaveChangesAsync();
        }

        public async Task<BudgetContainer?> GetByIdAsync (Guid id)
        {
            return await _context.BudgetContainers
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<BudgetContainer>> GetAllAsync()
        {
            if (_context == null)
            {
                throw new InvalidOperationException("BudgetTrackerDbContext is not initialized.");
            }

            return await _context.BudgetContainers
                .Include(b => b.Items)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(BudgetContainer budget)
        {
            _context.BudgetContainers.Update(budget);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var budget = await _context.BudgetContainers.FindAsync(id);
            if (budget == null)
                return false;

            _context.BudgetContainers.Remove(budget);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
