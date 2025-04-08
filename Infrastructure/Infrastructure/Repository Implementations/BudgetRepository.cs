using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Infrastructure.RepositoryImplementations
{
    public class BudgetRepository : GenericRepository<BudgetContainer>, IBudgetRepository
    {
        private readonly BudgetTrackerDbContext _context;
        private readonly ILogger<BudgetRepository> _logger;

        public BudgetRepository(BudgetTrackerDbContext context, ILogger<BudgetRepository> logger)
            : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public BudgetRepository(BudgetTrackerDbContext context) : base(context)
        {
        }

        public async Task AddAsync(BudgetContainer budget)
        {
            await _context.BudgetContainers.AddAsync(budget);
            await _context.SaveChangesAsync();
        }

        public async Task<BudgetContainer?> GetByIdAsync (Guid id)
        {
            return await _context.BudgetContainers
                .Include(b => b.BudgetItems)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<BudgetContainer>> GetAllAsync()
        {
            if (_context == null)
            {

                throw new InvalidOperationException("BudgetTrackerDbContext is not initialized.");
            }

            return await _context.BudgetContainers
                .Include(b => b.BudgetItems)
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
