using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BudgetTracker.Infrastructure.ExternalServices;
using Xunit;

namespace BudgetTracker.Tests.IntegrationTests
{
    // Helper class for faking HTTP responses.
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }

    public class CurrencyConversionServiceTests
    {
        [Fact]
        public async Task GetConversionRateAsync_ReturnsExpectedRate()
        {
            var jsonResponse = "{\"rates\":{\"EUR\":0.85}}";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse)
            };

            var fakeHandler = new FakeHttpMessageHandler(responseMessage);
            var client = new HttpClient(fakeHandler);
            var conversionService = new CurrencyConversionService(client);

            var rate = await conversionService.GetConversionRateAsync("USD", "EUR");
            Assert.Equal(0.85m, rate);
        }
    }
}
