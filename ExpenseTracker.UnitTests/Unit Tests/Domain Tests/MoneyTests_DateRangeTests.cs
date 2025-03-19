using System;
using BudgetTracker.Domain.ValueObjects;
using Xunit;

namespace BudgetTracker.Tests.UnitTests.DomainTests
{
    public class MoneyTests
    {
        [Fact]
        public void Add_WithSameCurrency_ReturnsCorrectSum()
        {
            // Arrange
            var money1 = new Money(100, "USD");
            var money2 = new Money(50, "USD");

            // Act
            var result = money1.Add(money2);

            // Assert
            Assert.Equal(150, result.Amount);
            Assert.Equal("USD", result.Currency);
        }

        [Fact]
        public void Add_WithDifferentCurrency_ThrowsInvalidException()
        {
            // Arrange
            var money1 = new Money(100, "USD");
            var money2 = new Money(200, "PLN");

            // Act
            var result = money1.Add(money2);

            // Assert
            Assert.Throws<InvalidOperationException> (() => money1.Add(money2));
        }
    }

    public class DateRangeTests
    {
        public static void DateRange_WithInvalidDates_ThrowsArgument()
        {
            Assert.Throws<ArgumentException>(() => new DateRange(DateTime.Today, DateTime.Today.AddDays(-1)));
        }
    }
}
