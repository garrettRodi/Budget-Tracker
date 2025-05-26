using System;
using System.Threading.Tasks;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.DataAccess;
using BudgetTracker.Infrastructure.RepositoryImplementations;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly BudgetTrackerDbContext _context;
        private readonly ILoggerFactory _loggerFactory;

        private IExpenseRepository? _expenseRepository;
        private IIncomeRepository? _incomeRepository;
        private IBudgetRepository? _budgetRepository;
        private ISavingGoalsRepository? _savingGoalsRepository;
        private ICategoryMappingRepository? _categoryMappingRepository;
        private IPlannedExpenseRepository? _plannedExpenseRepository;

        public UnitOfWork(BudgetTrackerDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _loggerFactory = loggerFactory;
        }

        public IExpenseRepository ExpenseRepository
            => _expenseRepository ??= new ExpenseRepository(_context, _loggerFactory.CreateLogger<ExpenseRepository>());

        public IIncomeRepository IncomeRepository
            => _incomeRepository ??= new IncomeRepository(_context, _loggerFactory.CreateLogger<IncomeRepository>());

        public IBudgetRepository BudgetRepository
            => _budgetRepository ??= new BudgetRepository(_context, _loggerFactory.CreateLogger<BudgetRepository>());

        public ISavingGoalsRepository SavingGoalsRepository
            => _savingGoalsRepository ??= new SavingGoalsRepository(_context, _loggerFactory.CreateLogger<SavingGoalsRepository>());

        public ICategoryMappingRepository CategoryMappingRepository
            => _categoryMappingRepository ??= new CategoryMappingRepository(_context, _loggerFactory.CreateLogger<CategoryMappingRepository>());

        public IPlannedExpenseRepository PlannedExpenseRepository
            => _plannedExpenseRepository ??= new PlannedExpenseRepository( _context,_loggerFactory.CreateLogger<PlannedExpenseRepository>());
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
