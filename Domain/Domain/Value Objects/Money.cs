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
            Currency = currency.Trim().ToUpperInvariant(); ;
        }
        private Money() { } // For EF Core

        // ——— NEW private constructor to allow negative values ———
        // This is used only by operator- below—no external code can call it.
        private Money(decimal amount, string currency, bool allowNegative)
        {
            // Skip the "amount < 0" check here
            Amount = amount;
            Currency = currency.Trim().ToUpperInvariant();
        }

        public override string ToString()
        {
            return $"{Amount:C} {Currency}";
        }

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

            // Use the private constructor that accepts a negative result:
            return new Money(a.Amount + b.Amount, a.Currency, allowNegative: true);
        }

        // Operator overload for -
        public static Money operator -(Money a, Money b)
        {
            if(a.Currency != b.Currency)
                throw new InvalidOperationException("Cannot subtract amounts in different currencies.");
            // Instead of using `new Money(a.Amount - b.Amount, a.Currency)` (which would throw
            // for negative), call the private constructor with allowNegative: true.
            return new Money(a.Amount - b.Amount, a.Currency, allowNegative: true);
        }
        public static Money operator /(Money m, decimal divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException();
            return new Money(m.Amount / divisor, m.Currency);
        }
    }
}