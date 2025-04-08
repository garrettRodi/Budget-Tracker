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
        

        public async Task AddAsync(Income income)
        {
            try
            {
                await _context.Incomes.AddAsync(income);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while adding the income.");
                throw new Exception("An error occurred while adding the income.", ex);
            }
        }

        public async Task<Income?> GetByIdAsync(Guid id)
        {
            return await _context.Incomes.FindAsync(id);
        }

        public async Task<IEnumerable<Income>> GetByBudgetContainerIdAsync(Guid budgetContainerId)
        {
            return await _context.Incomes
                .Where(i => i.BudgetContainerId == budgetContainerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Income>> GetAllAsync()
        {
            return await _context.Incomes.ToListAsync();
        }

        public async Task<bool> UpdateAsync(Income income)
        {
            _context.Incomes.Update(income);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var income = await _context.Incomes.FindAsync(id);
            if (income == null)
                return false;

            _context.Incomes.Remove(income);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
