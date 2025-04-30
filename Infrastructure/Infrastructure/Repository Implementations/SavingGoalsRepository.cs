using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BudgetTracker.Infrastructure.RepositoryImplementations
{
    public class SavingGoalsRepository : GenericRepository<SavingGoals>, ISavingGoalsRepository
    {
        private readonly BudgetTrackerDbContext _context;
        private readonly ILogger<SavingGoalsRepository> _logger;

        public SavingGoalsRepository(
             BudgetTrackerDbContext context,
             ILogger<SavingGoalsRepository> logger)
             : base(context)
        {
            _context = context;
            _logger = logger;
        }

        // Test ctor: allows `new SavingGoalsRepository(context)` in tests
        public SavingGoalsRepository(BudgetTrackerDbContext context)
            : this(context, NullLogger<SavingGoalsRepository>.Instance)
        {
        }
        public async Task<IEnumerable<SavingGoals>> GetSavingGoalsByBudgetContainerIdAsync(Guid budgetContainerId)
        {
            return await _dbSet
                .Where(g => g.BudgetContainerId == budgetContainerId)
                .ToListAsync();
        }
    }
}
