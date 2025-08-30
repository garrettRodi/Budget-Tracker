// File: 02 - Application/Application/Services/CurrencyService.cs

using System;
using System.Net.Http;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyConversionService _api;

        public CurrencyService(ICurrencyConversionService api)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            CurrentCurrency = "USD"; // Default currency
        }

        public string CurrentCurrency { get; set; }

        public Task SetCurrencyAsync(string newCurrency)
        {
            CurrentCurrency = newCurrency.ToUpperInvariant();
            return Task.CompletedTask;
        }

        public Task<decimal> GetConversionRateAsync(string from, string to)
            => _api.GetConversionRateAsync(from, to);
        public async Task<bool> IsSupportedCurrencyAsync(string supportedCode)
        {
            try
            {
                await _api.GetConversionRateAsync(CurrentCurrency, supportedCode);
                return true;
            }
            catch (HttpRequestException)
            {
                // Infrastructure-level HTTP errors indicate unsupported currency
                return false;
            }
        }

        public async Task<Money> ConvertAsync(Money original)
        {
            if (original.Currency == CurrentCurrency)
                return original;

            decimal rate = await GetConversionRateAsync(original.Currency, CurrentCurrency);
            return new Money(original.Amount * rate, CurrentCurrency);
        }
    }
}
