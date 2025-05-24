using System;
using System.Threading.Tasks;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.DataAccess;
using BudgetTracker.Tests.UnitTests.ApplicationTests;

namespace BudgetTracker.Tests.UnitTests.Fakes
{
    public class FakeUnitOfWork : IUnitOfWork
    {
        private readonly BudgetTrackerDbContext _context;
        public FakeUnitOfWork(BudgetTrackerDbContext context)
        {
            _context = context;
        }
        public IExpenseRepository ExpenseRepository { get; }
        public IIncomeRepository IncomeRepository { get; }
        public IBudgetRepository BudgetRepository { get; }
        public ISavingGoalsRepository SavingGoalsRepository { get; }
        public ICategoryMappingRepository CategoryMappingRepository { get; }
        public IPlannedExpenseRepository PlannedExpenseRepository { get; }

        public FakeUnitOfWork()
        {
            // Initialize with fake repositories.
            ExpenseRepository = new FakeExpenseRepository();
            IncomeRepository = new FakeIncomeRepository();
            BudgetRepository = new FakeBudgetRepository();
            SavingGoalsRepository = new FakeSavingGoalsRepository();
            CategoryMappingRepository = new FakeCategoryMappingRepository();
        }

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
