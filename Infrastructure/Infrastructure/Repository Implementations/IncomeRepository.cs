using BudgetTracker.Domain.Entities;
using BudgetTracker.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Domain.Interfaces;

namespace BudgetTracker.Infrastructure.RepositoryImplementations
{
    public class IncomeRepository : GenericRepository<Income>, IIncomeRepository
    {
        private readonly BudgetTrackerDbContext _context;
        
        public IncomeRepository(BudgetTrackerDbContext context)
            : base(context)
        { }
        

        public async Task AddAsync(Income income)
        {
            await _context.Incomes.AddAsync(income);
            await _context.SaveChangesAsync();
        }

        public async Task<Income?> GetByIdAsync(Guid id)
        {
            return await _context.Incomes.FindAsync(id);
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
