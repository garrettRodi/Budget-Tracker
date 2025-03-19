using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.DataAccess;
using BudgetTracker.Application.Services;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Tests.UnitTests.Fakes;
using Xunit;

namespace BudgetTracker.Tests.UnitTests.ApplicationTests
{
    public class ExpenseServiceTests
    {
        private readonly IExpenseService _expenseService;
        private readonly IUnitOfWork _fakeUnitOfWork;

        public ExpenseServiceTests()
        {
            _fakeUnitOfWork = new FakeUnitOfWork();
            _expenseService = new ExpenseService(_fakeUnitOfWork);
        }

        [Fact]
        public async Task CreateExpenseAsync_CreatesAndReturnsExpenseDTO()
        {
            // Arrange
            var createCommand = new CreateExpenseCommand
            {
                Name = "Test Expense",
                Amount = 50m,
                Date = DateTime.Now,
                Category = "Test Category"
            };

            // Act
            var expenseDto = await _expenseService.CreateExpenseAsync(createCommand);

            // Assert
            Assert.NotNull(expenseDto);
            Assert.Equal(createCommand.Name, expenseDto.Name);
            Assert.Equal(createCommand.Amount, expenseDto.Amount);
            Assert.Equal(createCommand.Date, expenseDto.Date);
            Assert.Equal(createCommand.Category, createCommand.Category);
        }

        [Fact]
        public async Task GetExpenseAsync_ReturnsAllExpenses()
        {
            // Arange
            var createCommand = new CreateExpenseCommand
            {
                Name = "Expense 1",
                Amount = 10m,
                Date = DateTime.Now,
                Category = "Category 1"
            };

            await _expenseService.CreateExpenseAsync(createCommand);

            // Act
            var expenses = await _expenseService.GetExpenseAsync();

            // Assert
            Assert.NotEmpty(expenses);
            Assert.Equal("Expense 1", expenses.First().Name);
        }

        [Fact]
        public async Task DeleteExpenseAsync_RemovesExpenseSuccessfully()
        {
            // Arrange
            var createCommand = new CreateExpenseCommand
            {
                Name = "Expense to Delete",
                Amount = 30m,
                Date = DateTime.Now,
                Category = "Category 2"
            };

            var createdExpense = await _expenseService.CreateExpenseAsync(createCommand);

            // Act
            var deleteResult = await _expenseService.DeleteExpenseAsync(createdExpense.Id);
            var retrievedExpense = await _expenseService.GetExpenseByIdAsync(createdExpense.Id);

            // Assert
            Assert.True(deleteResult);
            Assert.Null(retrievedExpense);

        }

        [Fact]
        public async Task ExpenseService_CreateExpense_UsesUnitOfWorkSuccessfully()
        {
            // Arrange: Configure an in-memory DbContext and UnitOfWork.
            var options = new DbContextOptionsBuilder<BudgetTrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new BudgetTrackerDbContext(options);
            var unitOfWork = new UnitOfWork(context);
            var expenseService = new ExpenseService(unitOfWork);

            var createCommand = new CreateExpenseCommand
            {
                Name = "Service Test Expense",
                Amount = 150m,
                Date = DateTime.Now,
                Category = "Test"
            };

            // Act
            var expenseDto = await expenseService.CreateExpenseAsync(createCommand);

            // Assert
            Assert.NotNull(expenseDto);
            Assert.Equal("Service Test Expense", expenseDto.Name);
            // Also verify that the expense was persisted via the UnitOfWork
            var expenseInDb = await context.Expenses.FindAsync(expenseDto.Id);
            Assert.NotNull(expenseInDb);
        }

    }
}
