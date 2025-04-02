using System;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Services;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Tests.UnitTests.Fakes;
using Xunit;

namespace BudgetTracker.Tests.UnitTests.ApplicationTests
{
    public class BudgetServiceTests
    {
        private readonly IBudgetService _budgetService;
        private readonly FakeUnitOfWork _fakeUnitOfWork;

        public BudgetServiceTests()
        {
            // FakeUnitOfWork should be updated to return a FakeBudgetRepository for IBudgetRepository.
            _fakeUnitOfWork = new FakeUnitOfWork();
            _budgetService = new BudgetService(_fakeUnitOfWork, new FakeBudgetLogger());
        }

        [Fact]
        public async Task CreateBudgetAsync_CreatesBudgetSuccessfully()
        {
            // Arrange
            var command = new CreateBudgetCommand
            {
                Name = "Test Budget",
                Frequency = BudgetFrequency.Monthly,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1),
                AutoRenew = false
            };

            // Act
            var result = await _budgetService.CreateBudgetAsync(command);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Budget", result.Name);
            // You could also check that the returned DTO includes an ID and correct dates.
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task GetAllBudgetsAsync_ReturnsAllCreatedBudgets()
        {
            // Arrange
            var command1 = new CreateBudgetCommand
            {
                Name = "Budget 1",
                Frequency = BudgetFrequency.Monthly,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1),
                AutoRenew = false
            };

            var command2 = new CreateBudgetCommand
            {
                Name = "Budget 2",
                Frequency = BudgetFrequency.Weekly,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(7),
                AutoRenew = true
            };

            await _budgetService.CreateBudgetAsync(command1);
            await _budgetService.CreateBudgetAsync(command2);

            // Act
            var budgets = await _budgetService.GetAllBudgetsAsync();

            // Assert
            Assert.NotEmpty(budgets);
            Assert.Equal(2, budgets.Count());
        }

        [Fact]
        public async Task UpdateBudgetAsync_UpdatesBudgetSuccessfully()
        {
            // Arrange
            var createCommand = new CreateBudgetCommand
            {
                Name = "Original Budget",
                Frequency = BudgetFrequency.Monthly,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1),
                AutoRenew = false
            };

            var createdBudget = await _budgetService.CreateBudgetAsync(createCommand);

            // Act: Update the budget details.
            var updateCommand = new UpdateBudgetCommand
            {
                Id = createdBudget.Id,
                Name = "Updated Budget",
                Frequency = BudgetFrequency.Yearly,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
                AutoRenew = true
            };

            bool updateResult = await _budgetService.UpdateBudgetAsync(updateCommand);
            var updatedBudget = await _budgetService.GetBudgetByIdAsync(createdBudget.Id);

            // Assert
            Assert.True(updateResult);
            Assert.NotNull(updatedBudget);
            Assert.Equal("Updated Budget", updatedBudget.Name);
            Assert.Equal(BudgetFrequency.Yearly, updatedBudget.Frequency);
            Assert.True(updatedBudget.AutoRenew);
        }

        [Fact]
        public async Task DeleteBudgetAsync_DeletesBudgetSuccessfully()
        {
            // Arrange
            var createCommand = new CreateBudgetCommand
            {
                Name = "Budget To Delete",
                Frequency = BudgetFrequency.Monthly,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1),
                AutoRenew = false
            };

            var createdBudget = await _budgetService.CreateBudgetAsync(createCommand);

            // Act
            bool deleteResult = await _budgetService.DeleteBudgetAsync(createdBudget.Id);
            var budgetAfterDeletion = await _budgetService.GetBudgetByIdAsync(createdBudget.Id);

            // Assert
            Assert.True(deleteResult);
            Assert.Null(budgetAfterDeletion);
        }
    }
}
