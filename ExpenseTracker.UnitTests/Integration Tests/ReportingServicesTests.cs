using System;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Application.Services;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Infrastructure.DataAccess;
using BudgetTracker.Tests.UnitTests.Fakes; // Assuming you have FakeUnitOfWork, FakeCategoryMappingService, etc.
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BudgetTracker.Tests.IntegrationTests
{
    public class ReportingServicesTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryMappingService _categoryMappingService;
        private readonly IReportingService _reportingService;
        public ReportingServicesTests()
        {
            // Uses in-memory database and fake services for testing.
            var opptions = new DbContextOptionsBuilder<BudgetTracker.Infrastructure.DataAccess.BudgetTrackerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new BudgetTracker.Infrastructure.DataAccess.BudgetTrackerDbContext(opptions);
            _unitOfWork = new FakeUnitOfWork(context);
            _categoryMappingService = new FakeCategoryMappingService();
            _reportingService = new ReportingService(_unitOfWork, _categoryMappingService);
        }

        [Fact]
        public async Task GenerateExpenseReportAsync_ShouldReturnCorrectReport()
        {
            // Arrange
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2023, 1, 31);
            await _unitOfWork.ExpenseRepository.AddAsync(new Expense
            {
                Amount = 100,
                Category = "Food",
                ExpenseDate = new DateTime(2023, 1, 15)
            });
            await _unitOfWork.ExpenseRepository.AddAsync(new Expense
            {
                Amount = 50,
                Category = "Transport",
                ExpenseDate = new DateTime(2023, 1, 20)
            });
            await _unitOfWork.CommitAsync();
            // Act
            var report = await _reportingService.GenerateExpenseReportAsync(startDate, endDate);
            // Assert
            Assert.Equal(150, report.TotalExpenses);
            Assert.Equal(100, report.CategoryTotals["Food"]);
            Assert.Equal(50, report.CategoryTotals["Transport"]);
        }
    }
}
