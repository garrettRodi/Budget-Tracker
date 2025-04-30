using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BudgetTracker.Infrastructure.RepositoryImplementations
{
    public class BudgetRepository : GenericRepository<BudgetContainer>, IBudgetRepository
    {
        private readonly BudgetTrackerDbContext _context;
        private readonly ILogger<BudgetRepository> _logger;

        public BudgetRepository(
             BudgetTrackerDbContext context,
             ILogger<BudgetRepository> logger)
             : base(context)
        {
            _context = context;
            _logger = logger;
        }

        // Test ctor: allows `new BudgetRepository(context)` in tests
        public BudgetRepository(BudgetTrackerDbContext context)
            : this(context, NullLogger<BudgetRepository>.Instance)
        {
        }

        // Include BudgetItems when fetching one
        public override async Task<BudgetContainer> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(b => b.BudgetItems)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        // Include BudgetItems on list
        public override async Task<IEnumerable<BudgetContainer>> GetAllAsync()
        {
            return await _dbSet
                .Include(b => b.BudgetItems)
                .ToListAsync();
        }
    }
}
