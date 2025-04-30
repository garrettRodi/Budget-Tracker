using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Infrastructure.RepositoryImplementations
{
    public class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
    {
        private readonly BudgetTrackerDbContext _context;
        private readonly ILogger<ExpenseRepository> _logger;
        public ExpenseRepository(BudgetTrackerDbContext context, ILogger<ExpenseRepository> logger)
            : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public ExpenseRepository(BudgetTrackerDbContext context) : base(context)
        {
            // you can store the logger if you want to log here
        }

        public async Task<IEnumerable<Expense>> GetExpensesByBudgetContainerIdAsync(Guid budgetContainerId)
        {
            return await _dbSet
                .Where(e => e.BudgetContainerId == budgetContainerId)
                .ToListAsync();
        }
    }
}
