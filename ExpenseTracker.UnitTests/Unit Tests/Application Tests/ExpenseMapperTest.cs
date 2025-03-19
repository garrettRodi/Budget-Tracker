using System;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Mappers;
using BudgetTracker.Domain.Entities;
using Microsoft.Extensions.DependencyModel;
using Xunit;

namespace BudgetTracker.Tests.UnitTests.ApplicationTests
{
   public  class ExpenseMapperTest
    {
        [Fact]
        public void ToEntity_FromCreateExpenseCommand_MapsPropertiesCorrectly()
        {
            // Arrange
            var command = new CreateExpenseCommand
            {
                Name = "Pie",
                Amount = 15.5m,
                Date = new DateTime(2025, 2, 28),
                Category = "Food"
            };

            // Act
            var expense = command.ToEntity();

            // Assert
            Assert.Equal("Pie", expense.Name);
            Assert.Equal(15.5m, expense.Amount);
            Assert.Equal(new DateTime(2025, 2, 28), expense.ExpenseDate);
            Assert.Equal("Food", expense.Category);
            Assert.NotEqual(Guid.Empty, expense.Id);
        }

        [Fact]
        public void ToEntity_FromUpdateExpenseCommand_UpdatesExistingExpenseCorrectly()
        {
            // Arrange
            var existingExpense = new Expense
            {
                Id = Guid.NewGuid(),
                Name = "Apple",
                Amount = 50.0m,
                ExpenseDate = new DateTime(2025, 2, 27),
                Category = "Food"
            };

            var updateCommand = new UpdateExpenseCommand
            {
                Id = existingExpense.Id,
                Name = "Apple Updated",
                Amount = 5.0m,
                Date = new DateTime(2025, 2, 28),
                Category = "Food"
            };

            // Act
            var updateExpense = updateCommand.ToEntity(existingExpense);

            // Assert
            Assert.Equal(existingExpense.Id, updateExpense.Id);
            Assert.Equal("Apple Updated", updateExpense.Name);
            Assert.Equal(5.0m, updateExpense.Amount);
            Assert.Equal(new DateTime(2025, 2, 28), updateExpense.ExpenseDate);
            Assert.Equal("Food", updateExpense.Category);
        }
    }
}
