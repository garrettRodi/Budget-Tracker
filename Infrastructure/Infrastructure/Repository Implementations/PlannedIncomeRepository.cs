using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Infrastructure.RepositoryImplementations
{
    public class PlannedIncomeRepository : IPlannedIncomeRepository
    {
        private readonly BudgetTrackerDbContext _context;
        private readonly ILogger<PlannedIncomeRepository> _logger;

        public PlannedIncomeRepository(
            BudgetTrackerDbContext context,
            ILogger<PlannedIncomeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(PlannedIncome plannedIncome)
        {
            await _context.PlannedIncomes.AddAsync(plannedIncome);
            await _context.SaveChangesAsync();
        }

        public async Task<PlannedIncome?> GetByIdAsync(Guid id)
            => await _context.PlannedIncomes.FindAsync(id);

        public async Task<IEnumerable<PlannedIncome>> FindByBudgetAsync(Guid budgetContainerId)
            => await _context.PlannedIncomes
            .Where(pi => pi.BudgetContainerId == budgetContainerId)
            .ToListAsync();

        public async Task<bool> UpdateAsync(PlannedIncome plannedIncome)
        {
            _context.PlannedIncomes.Update(plannedIncome);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.PlannedIncomes.FindAsync(id);
            if (entity == null) return false;
            _context.PlannedIncomes.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
