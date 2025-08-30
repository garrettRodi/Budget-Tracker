using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.Interfaces
{
    public interface ICurrencyService
    {
        string CurrentCurrency { get; set; }

        // for SettingsMenu
        Task SetCurrencyAsync(string newCode);

        // optional Helper method to check if a currency code is supported
        Task<bool> IsSupportedCurrencyAsync(string supportedCode);
        
        // Converts a Money object from its native currency to the current display currency.
        Task<Money> ConvertAsync(Money original);

        // Fetches the latest conversion rates
        Task<decimal> GetConversionRateAsync(string fromCurrency, string toCurrency);
    }
}
