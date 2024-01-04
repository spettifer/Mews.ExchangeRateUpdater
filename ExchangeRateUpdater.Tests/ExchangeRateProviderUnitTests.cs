
using ExchangeRateUpdater.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;

namespace ExchangeRateUpdater.Tests;

public class ExchangeRateProviderUnitTests : IClassFixture<ExchangeRateData>
{
    private readonly ExchangeRateData _fixture;

    public ExchangeRateProviderUnitTests(ExchangeRateData exchangeRateData)
    {
        _fixture = exchangeRateData;
    }
    
    [Fact]
    public async Task GivenCurrenciesInFeed_Then_ReturnsRatesForThoseCurrencies()
    {
        const string mockBaseUrl = "http://mock.url/*";
        var mockHttp = new MockHttpMessageHandler();
        
        mockHttp.When(mockBaseUrl)
            .Respond("application/json", _fixture.ApiResponse);
        
        var mockSettings = Options.Create(new ApiSettings() { LanguageCode = "EN", ExchangeRatesBaseUrl = mockBaseUrl, DailyRatesEndpoint = "daily" });

        var sut = new ExchangeRateProvider(mockHttp.ToHttpClient(), mockSettings);

        // Ask for the two rates that are in the test data where the currency code starts with B.
        var rates = await sut.GetExchangeRates(_fixture.CurrenciesInTestData.Where(c => c.Code.StartsWith("B")));

        rates.Should().HaveCount(2);
    }
}