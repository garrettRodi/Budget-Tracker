using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Application.Interfaces;

namespace BudgetTracker.Presentation.PresentationHelpers
{
    public static class MoneyConversionExtension
    {
        /// <summary>
        /// Converts a USD-based Money into the user’s CurrentCurrency,
        /// then formats it with the appropriate symbol and code.
        /// </summary>
        public static string ToDisplay(this Money original, ICurrencyService cs)
        {
            var conv = cs.Convert(original);
            return $"{conv.Amount:C} ({conv.Currency})";
                }
    }
}
