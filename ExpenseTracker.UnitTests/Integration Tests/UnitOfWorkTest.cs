using System;
using System.Threading.Tasks;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Infrastructure.DataAccess;
using BudgetTracker.Infrastructure.RepositoryImplementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;

namespace BudgetTracker.Tests.IntegrationTests
{
    public class UnitOfWorkTests : IDisposable
    {
        private readonly BudgetTrackerDbContext _context;
        private readonly UnitOfWork _unitOfWork;

        public UnitOfWorkTests()
        {
            // Configure an in-memory database
            var options = new DbContextOptionsBuilder<BudgetTrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new BudgetTrackerDbContext(options);
            var loggerFactory = LoggerFactory.Create(builder => builder.AddDebug());
            _unitOfWork = new UnitOfWork(_context, loggerFactory);

        }

        [Fact]
        public async Task UnitOfWork_ShouldCommitExpenseAddition()
        {
            // Arrange
            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                Name = "UoW Test Expense",
                Amount = 200m,
                ExpenseDate = DateTime.Now,
                Category = "Test"
            };

            // Act
            await _unitOfWork.ExpenseRepository.AddAsync(expense);
            await _unitOfWork.CommitAsync();

            // Assert: retrieve the expense via context to verify it was committed.
            var retrievedExpense = await _context.Expenses.FindAsync(expense.Id);
            Assert.NotNull(retrievedExpense);
            Assert.Equal(expense.Name, retrievedExpense.Name);
        }

        [Fact]
        public async Task UnitOfWork_ShouldExposeIncomeRepository()
        {
            // Arrange & Act: Get IncomeRepository instance from UnitOfWork.
            var incomeRepo = _unitOfWork.IncomeRepository;

            // Assert: incomeRepo should not be null and be of the correct type.
            Assert.NotNull(incomeRepo);
            Assert.IsType<IncomeRepository>(incomeRepo);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
