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
        string CurrentCurrency { get; }

        // Switches your app’s currency (and fetches the new rate USD→NewCurrency)
        Task SetCurrencyAsync(string newCurrency);

        // Returns a new Money in the CurrentCurrency, converting the amount as needed.
        Money Convert(Money original);
    }
}
