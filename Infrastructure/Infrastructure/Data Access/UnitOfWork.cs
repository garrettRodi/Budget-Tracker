using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.RepositoryImplementations;
using SQLitePCL;

namespace BudgetTracker.Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BudgetTrackerDbContext _context;
        private IExpenseRepository? _expenseRepository;
        private IIncomeRepository? _incomeRepository;
        private IBudgetRepository? _budgetRepository;
        private ISavingGoalsRepository? _savingGoalsRepository;
        private ICategoryMappingRepository? _categoryMappingRepository;

        public UnitOfWork(BudgetTrackerDbContext context)
        {
            _context = context;
        }

        public IExpenseRepository ExpenseRepository
            => _expenseRepository ??= new ExpenseRepository(_context);

        public IIncomeRepository IncomeRepository
            => _incomeRepository ??= new IncomeRepository(_context);

        public IBudgetRepository BudgetRepository
            => _budgetRepository ??= new BudgetRepository(_context);

        public ISavingGoalsRepository SavingGoalsRepository
            => _savingGoalsRepository ??= new SavingGoalsRepository(_context);

        public ICategoryMappingRepository CategoryMappingRepository
            => _categoryMappingRepository ??= new CategoryMappingRepository(_context);
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
