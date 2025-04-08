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
        }

        public async Task AddAsync(Expense expense)
        {
            try
            {
                await _context.Expenses.AddAsync(expense);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Optionally log the exception or rethrow with additional context.
                throw;
            }
        }

        public async Task<Expense?> GetByIdAsync(Guid id)
        {
            return await _context.Expenses.FindAsync(id);
        }

        public async Task<IEnumerable<Expense>> GetByBudgetContainerIdAsync(Guid budgetContainerId)
        {
            return await _context.Expenses
                .Where(e => e.BudgetContainerId == budgetContainerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Expense>> GetAllAsync()
        {
            return await _context.Expenses.ToListAsync();
        }

        public async Task<bool> UpdateAsync(Expense expense)
        {
            _context.Expenses.Update(expense);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return false;

            _context.Expenses.Remove(expense);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
