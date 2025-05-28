using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Application.Services;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Tests.UnitTests.Fakes; // For FakeUnitOfWork, FakeCategoryMappingService, etc.
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Tests.IntegrationTests
{
    public class ReportingServicesTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryMappingService _categoryMappingService;
        private readonly IReportingService _reportingService;
        private readonly ILogger<ReportingService> _logger;
        public ReportingServicesTests()
        {
            // Uses in-memory database and fake services for testing.
            var options = new DbContextOptionsBuilder<BudgetTracker.Infrastructure.DataAccess.BudgetTrackerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new BudgetTracker.Infrastructure.DataAccess.BudgetTrackerDbContext(options);
            _unitOfWork = new FakeUnitOfWork(context);
            _categoryMappingService = new FakeCategoryMappingService();
            _reportingService = new ReportingService(_unitOfWork, _categoryMappingService, _logger);
        }

        [Fact]
        public async Task GenerateExpenseReportAsync_ShouldReturnCorrectReport()
        {
            // Arrange
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2023, 1, 31);

            // Create a budget container and commit it.
            var budgetContainer = new BudgetContainer
            {
                Id = Guid.NewGuid(),
                Name = "Test Budget",
                Frequency = BudgetFrequency.Monthly,
                StartDate = startDate,
                EndDate = endDate,
                AutoRenew = false,
                BudgetItems = new List<BudgetItem>()
            };
            await _unitOfWork.BudgetRepository.AddAsync(budgetContainer);
            await _unitOfWork.CommitAsync();

            // Use the created budget container's Id.
            Guid activeBudgetId = budgetContainer.Id;

            // Now add two expenses that belong to that budget container.
            await _unitOfWork.ExpenseRepository.AddAsync(new Expense
            {
                Amount = 100,
                Category = "Food",
                ExpenseDate = new DateTime(2023, 1, 15),
                BudgetContainerId = activeBudgetId
            });
            await _unitOfWork.ExpenseRepository.AddAsync(new Expense
            {
                Amount = 50,
                Category = "Transport",
                ExpenseDate = new DateTime(2023, 1, 20),
                BudgetContainerId = activeBudgetId
            });
            await _unitOfWork.CommitAsync();

            // Act: Generate the expense report for the active budget container over the given dates.
            var report = await _reportingService.GenerateExpenseReportAsync(activeBudgetId, startDate, endDate);

            // Assert
            Assert.Equal(150, report.TotalExpenses);
            Assert.Equal(100, report.CategoryTotals["Food"]);
            Assert.Equal(50, report.CategoryTotals["Transport"]);
        }
    }
}
