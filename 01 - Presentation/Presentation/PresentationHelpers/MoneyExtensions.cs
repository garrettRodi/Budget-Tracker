// File: 01 - Presentation/Presentation/PresentationHelpers/MoneyExtensions.cs
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Presentation.PresentationHelpers
{
    public static class MoneyExtensions
    {
        public static string ToDisplay(this Money money)
        {
            // You can tweak the format here if you prefer symbols or different placement.
            return $"{money.Currency} {money.Amount:N2}";
        }

        public static async Task<string> ToDisplayAsync(this Money money, ICurrencyService currencyService)
        {
            var converted = await currencyService.ConvertAsync(money);
            return converted.ToDisplay();
        }
    }
}
