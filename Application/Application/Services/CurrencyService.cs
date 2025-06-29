using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Infrastructure.ExternalServices;

namespace BudgetTracker.Application.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly CurrencyConversionService _api;
        public string CurrentCurrency { get; private set; } = "USD";
            private decimal _conversionRate = 1.0m; // USD -> CurrentCurrency

        public CurrencyService(CurrencyConversionService api)
        {
            _api = api;
        }

        public async Task SetCurrencyAsync(string newCurrency)
        {
            if (string.Equals(newCurrency, CurrentCurrency, StringComparison.OrdinalIgnoreCase))
                return;

            // Fetch the conversion rate from USD to the new currency
            _conversionRate = await _api.GetConversionRateAsync("USD", newCurrency);
            CurrentCurrency = newCurrency.ToUpperInvariant();
        }

        public Money Convert(Money original)
        {
            // If its already in the current currency, return it as is
            if (string.Equals(original.Currency, CurrentCurrency, StringComparison.OrdinalIgnoreCase))
                return original;

            // otherwise, convert it
            var amountInUsd = original.Amount;
            var converted = amountInUsd * _conversionRate;
            return new Money(converted, CurrentCurrency);
        }
    }
}
