// ... existing using statements ...

namespace BudgetTracker.Domain.ValueObjects
{
    public class Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative.");
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required");

            Amount = amount;
            Currency = currency;
        }
        private Money() { } // For EF Core

        public Money Add(Money other)
        {
            if (other.Currency != Currency)
                throw new InvalidOperationException("Cannot add amounts in different currency.");

            return new Money(Amount + other.Amount, Currency);
        }

        // Operator overload for +
        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException("Cannot add amounts in different currencies.");
            return new Money(a.Amount + b.Amount, a.Currency);
        }

        // Operator overload for -
        public static Money operator -(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException("Cannot subtract amounts in different currencies.");
            return new Money(a.Amount - b.Amount, a.Currency);
        }
        public static Money operator /(Money m, decimal divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException();
            return new Money(m.Amount / divisor, m.Currency);
        }
    }
}