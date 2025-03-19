using System;
using System.Threading.Tasks;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Infrastructure.DataAccess;
using BudgetTracker.Infrastructure.RepositoryImplementations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BudgetTracker.Tests.IntegrationTests
{
    public class ExpenseRepositoryIntegrationTests : IDisposable
    {
        private readonly BudgetTrackerDbContext _context;
        private readonly ExpenseRepository _repository;

        public ExpenseRepositoryIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<BudgetTrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new BudgetTrackerDbContext(options);
            _repository = new ExpenseRepository(_context);
        }
        [Fact]
        public async Task AddAndRetrieveExpense_Succeeds()
        {
            // Arrange
            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                Name = "Integration Test Expense",
                Amount = 100m,
                ExpenseDate = DateTime.Now,
                Category = "Test"
            };

            // Act
            await _repository.AddAsync(expense);
            var retrievedExpense = await _repository.GetByIdAsync(expense.Id);

            // Assert 
            Assert.NotNull(retrievedExpense);
            Assert.Equal(expense.Name, retrievedExpense.Name);
        }


        [Fact]
        public async Task UpdateExpense_Succeeds()
        {
            // Arrange
            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                Name = "Initial Name",
                Amount = 50m,
                ExpenseDate = DateTime.Now,
                Category = "Test"
            };

            await _repository.AddAsync(expense);

            // Act
            expense.Name = "Updated Name";
            bool updateResult = await _repository.UpdateAsync(expense);
            var updatedExpense = await _repository.GetByIdAsync(expense.Id);

            // Assert
            Assert.True(updateResult);
            Assert.Equal("Updated Name", updatedExpense.Name);
        }

        [Fact]
        public async Task DeleteExpense_Succeeds()
        {
            // Arrange
            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                Name = "Expense To Delete",
                Amount = 75m,
                ExpenseDate = DateTime.Now,
                Category = "Test"
            };

            await _repository.AddAsync(expense);

            // Act
            bool deleteResult = await _repository.DeleteAsync(expense.Id);
            var retrievedExpense = await _repository.GetByIdAsync(expense.Id);

            // Assert
            Assert.True(deleteResult);
            Assert.Null(retrievedExpense);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
