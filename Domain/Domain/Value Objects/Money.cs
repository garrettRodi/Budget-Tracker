using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

       public Money Add(Money other)
        {
            if (other.Currency != Currency)
                throw new InvalidOperationException("Cannot add amounts in different currency.");

            return new Money(Amount + other.Amount, Currency);
        }
    }
}
