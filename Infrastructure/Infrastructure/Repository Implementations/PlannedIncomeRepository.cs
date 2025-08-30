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
    public class PlannedIncomeRepository 
        : GenericRepository<PlannedIncome>, IPlannedIncomeRepository
    {
        private readonly ILogger<PlannedIncomeRepository> _logger;

        public PlannedIncomeRepository(
            BudgetTrackerDbContext context,
            ILogger<PlannedIncomeRepository> logger) : base(context)
        {
            _logger = logger;
        }
        public async Task<IEnumerable<PlannedIncome>> FindByBudgetAsync(Guid budgetContainerId)
        {
            return await _dbSet
           .Where(pi => pi.BudgetContainerId == budgetContainerId)
           .ToListAsync();
        }
    }
}
