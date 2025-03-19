using System;
using System.Threading.Tasks;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Infrastructure.DataAccess;
using BudgetTracker.Infrastructure.RepositoryImplementations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BudgetTracker.Tests.IntegrationTests
{
    public class BudgetRepositoryIntegrationTests : IDisposable
    {
        private readonly BudgetTrackerDbContext _context;
        private readonly BudgetRepository _repository;

        public BudgetRepositoryIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<BudgetTrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new BudgetTrackerDbContext(options);
            _repository = new BudgetRepository(_context);
        }

        [Fact]
        public async Task AddAndRetrieveBudget_Succeeds()
        {
            // Arrange
            var budgetContainer = new BudgetContainer
            {
                Id = Guid.NewGuid(),
                Name = "Test Budget",
                Frequency = BudgetFrequency.Monthly,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                AutoRenew = false
            };

            // Add an associated budget item.
            budgetContainer.Items.Add(new BudgetItem
            {
                Id = Guid.NewGuid(),
                BudgetContainerId = budgetContainer.Id,
                Category = "Food",
                PlannedAmount = 200m,
                ActualAmount = 0m
            });

            // Act
            await _repository.AddAsync(budgetContainer);
            var retrieved = await _repository.GetByIdAsync(budgetContainer.Id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal(budgetContainer.Name, retrieved.Name);
            Assert.Single(retrieved.Items);
        }

        [Fact]
        public async Task UpdateBudget_Succeeds()
        {
            // Arrange
            var budgetContainer = new BudgetContainer
            {
                Id = Guid.NewGuid(),
                Name = "Initial Budget",
                Frequency = BudgetFrequency.Monthly,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                AutoRenew = false
            };

            await _repository.AddAsync(budgetContainer);

            // Act: Update the name and commit the change.
            budgetContainer.Name = "Updated Budget";
            var updateResult = await _repository.UpdateAsync(budgetContainer);
            var retrieved = await _repository.GetByIdAsync(budgetContainer.Id);

            // Assert
            Assert.True(updateResult);
            Assert.Equal("Updated Budget", retrieved.Name);
        }

        [Fact]
        public async Task DeleteBudget_Succeeds()
        {
            // Arrange
            var budgetContainer = new BudgetContainer
            {
                Id = Guid.NewGuid(),
                Name = "Budget to Delete",
                Frequency = BudgetFrequency.Monthly,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                AutoRenew = false
            };

            await _repository.AddAsync(budgetContainer);

            // Act
            var deleteResult = await _repository.DeleteAsync(budgetContainer.Id);
            var retrieved = await _repository.GetByIdAsync(budgetContainer.Id);

            // Assert
            Assert.True(deleteResult);
            Assert.Null(retrieved);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
