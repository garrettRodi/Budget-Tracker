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
    public class PlannedExpenseRepository
        : GenericRepository<PlannedExpense>, IPlannedExpenseRepository
    {
        private readonly BudgetTrackerDbContext _context;
        private readonly ILogger<PlannedExpenseRepository> _logger;
        public PlannedExpenseRepository(
           BudgetTrackerDbContext context,
           ILogger<PlannedExpenseRepository> logger)
           : base(context)                   // ← Pass the context up to GenericRepository
        {
            _logger = logger;
        }

        public async Task<IEnumerable<PlannedExpense>> GetPlannedExpensesByBudgetAsync(Guid budgetContainerId)
        {
            return await _context.PlannedExpenses
                .Where(pe => pe.BudgetContainerId == budgetContainerId)
                .ToListAsync();
        }
    }
}
