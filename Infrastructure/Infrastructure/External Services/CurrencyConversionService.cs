using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BudgetTracker.Infrastructure.ExternalServices
{
    public class CurrencyConversionService
    {
        private readonly HttpClient _httpClient;

        public CurrencyConversionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetConversionRateAsync(string fromCurrency, string toCurrency)
        {
            // Example API endpoint using HTTPS
            string url = $"https://open.er-api.com/v6/latest/{fromCurrency}";
            var response = await _httpClient.GetStringAsync(url);

            using JsonDocument document = JsonDocument.Parse(response);
            JsonElement root = document.RootElement;
            JsonElement rates = root.GetProperty("rates");
            decimal rate = rates.GetProperty(toCurrency).GetDecimal();

            return rate;
        }
    }
}
