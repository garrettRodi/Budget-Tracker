using System;
using System.Threading.Tasks;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Tests.UnitTests.ApplicationTests;

namespace BudgetTracker.Tests.UnitTests.Fakes
{
    public class FakeUnitOfWork : IUnitOfWork
    {
        public IExpenseRepository ExpenseRepository { get; }
        public IIncomeRepository IncomeRepository { get; }
        public IBudgetRepository BudgetRepository { get; }
        public ISavingGoalsRepository SavingGoalsRepository { get; }
        public ICategoryMappingRepository CategoryMappingRepository { get; }

        public FakeUnitOfWork()
        {
            // Initialize with fake repositories.
            ExpenseRepository = new FakeExpenseRepository();
            IncomeRepository = new FakeIncomeRepository();
            BudgetRepository = new FakeBudgetRepository();
            SavingGoalsRepository = new FakeSavingGoalsRepository();
            CategoryMappingRepository = new FakeCategoryMappingRepository();
        }

        public Task<int> CommitAsync() => Task.FromResult(1);

        public void Dispose()
        {
            // Nothing to dispose in the fake implementation.
            GC.SuppressFinalize(this);
        }
    }
}
