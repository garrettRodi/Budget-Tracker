using BudgetTracker.Domain.Entities;
using BudgetTracker.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Infrastructure.RepositoryImplementations
{
    public class IncomeRepository : GenericRepository<Income>, IIncomeRepository
    {
        private readonly BudgetTrackerDbContext _context;
        private readonly ILogger<IncomeRepository> _logger;

        public IncomeRepository(BudgetTrackerDbContext context, ILogger<IncomeRepository> logger)
            : base(context)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<IEnumerable<Income>> GetIncomesByBudgetContainerIdAsync(Guid budgetContainerId)
        {
            return await _dbSet
                .Where(i => i.BudgetContainerId == budgetContainerId)
                .ToListAsync();
        }
    }
}
