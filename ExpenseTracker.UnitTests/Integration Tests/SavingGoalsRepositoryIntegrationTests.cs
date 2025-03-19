using System;
using System.Threading.Tasks;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Infrastructure.DataAccess;
using BudgetTracker.Infrastructure.RepositoryImplementations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BudgetTracker.Tests.IntegrationTests
{
    public class SavingGoalsRepositoryIntegrationTests : IDisposable
    {
        private readonly BudgetTrackerDbContext _context;
        private readonly SavingGoalsRepository _repository;

        public SavingGoalsRepositoryIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<BudgetTrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new BudgetTrackerDbContext(options);
            _repository = new SavingGoalsRepository(_context);
        }

        [Fact]
        public async Task AddAndRetrieveSavingGoal_Succeeds()
        {
            // Arrange
            var savingGoal = new SavingGoals
            {
                Id = Guid.NewGuid(),
                GoalName = "Emergency Fund",
                TargetAmount = 5000m,
                CurrentAmount = 1000m,
                TargetDate = DateTime.Now.AddYears(1)
            };

            // Act
            await _repository.AddAsync(savingGoal);
            var retrieved = await _repository.GetByIdAsync(savingGoal.Id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal(savingGoal.GoalName, retrieved.GoalName);
        }

        [Fact]
        public async Task UpdateSavingGoal_Succeeds()
        {
            // Arrange
            var savingGoal = new SavingGoals
            {
                Id = Guid.NewGuid(),
                GoalName = "Vacation Fund",
                TargetAmount = 3000m,
                CurrentAmount = 500m,
                TargetDate = DateTime.Now.AddMonths(6)
            };

            await _repository.AddAsync(savingGoal);

            // Act: Update the current amount.
            savingGoal.CurrentAmount = 1000m;
            var updateResult = await _repository.UpdateAsync(savingGoal);
            var retrieved = await _repository.GetByIdAsync(savingGoal.Id);

            // Assert
            Assert.True(updateResult);
            Assert.Equal(1000m, retrieved.CurrentAmount);
        }

        [Fact]
        public async Task DeleteSavingGoal_Succeeds()
        {
            // Arrange
            var savingGoal = new SavingGoals
            {
                Id = Guid.NewGuid(),
                GoalName = "Car Down Payment",
                TargetAmount = 8000m,
                CurrentAmount = 2000m,
                TargetDate = DateTime.Now.AddYears(2)
            };

            await _repository.AddAsync(savingGoal);

            // Act
            var deleteResult = await _repository.DeleteAsync(savingGoal.Id);
            var retrieved = await _repository.GetByIdAsync(savingGoal.Id);

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
