using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Interfaces
{
    public interface ICurrencyConversionService
    {
        Task<decimal> GetConversionRateAsync(string fromCurrency, string toCurrency);
    }
}
