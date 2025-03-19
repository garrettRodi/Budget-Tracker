using System;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Exceptions;
using BudgetTracker.Domain.Services;
using Xunit;

namespace BudgetTracker.Tests.UnitTests.DomainTests
{
    public class ExpenseValidatoraTests
    {
        [Fact]
        public void ValidateExpense_WithNegativeAmount_ThrowsInvalidExpenseException()
        {
            // Arrange
            var expense = new Expense
            {
                Amount = -10,
                    ExpenseDate = DateTime.Now
            };
            var validator = new ExpenseValidator();

            // Act & Assert
            Assert.Throws<InvalidExpenseException> (() => validator.ValidateExpense(expense));
        }

        [Fact]
        public void ValidateExpense_WithFutureDate_ThrowsException()
        {
            // Arrange
            var expense = new Expense
            {
                Amount = 100,
                ExpenseDate = DateTime.Now.AddDays(1)
            };
            var validator = new ExpenseValidator();

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => validator.ValidateExpense(expense));
            Assert.Contains("Expense date cannot be in the future", exception.Message);
        }

        [Fact]
        public void ValidateExpense_WithValidExpense_DoesNotThrow()
        {
            var expense = new Expense
            {
                // Arrange
                Amount = 100,
            ExpenseDate = DateTime.Now
            };
            var validator = new ExpenseValidator();

            // Act & Assert ( no exception should be thrown
            var exceptionRecord = Record.Exception(() => validator.ValidateExpense(expense));
            Assert.Null(exceptionRecord);
        }
    }
}
