using System.Net;
using ExchangeRateUpdater.Tests.Fixtures;
using FluentAssertions;
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
    public async Task GivenNullCurrencyList_Then_ReturnNoRates()
    {
        var sut = new ExchangeRateProvider(_fixture.MockHttpClient, _fixture.MockSettings);

        var rates = await sut.GetExchangeRates(null, _fixture.MockDate);
        
        rates.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GivenCurrenciesNotInFeed_Then_ReturnsNoRates()
    {
        var sut = new ExchangeRateProvider(_fixture.MockHttpClient, _fixture.MockSettings);
        
        var rates = await sut.GetExchangeRates(_fixture.CurrenciesNotInTestData, _fixture.MockDate);

        rates.Should().BeEmpty();
    } 
    
    [Fact]
    public async Task GivenCurrenciesInFeed_Then_ReturnsRatesForThoseCurrencies()
    {
        var sut = new ExchangeRateProvider(_fixture.MockHttpClient, _fixture.MockSettings);

        // Ask for the rates that are in the test data where the currency code starts with B.
        var currencies = _fixture.CurrenciesInTestData.Where(c => c.Code.StartsWith("B")).ToList();
        
        var rates = await sut.GetExchangeRates(currencies, _fixture.MockDate);

        rates.Should().HaveCount(currencies.Count);
    }
    
    [Fact]
    public async Task GivenMixOfCurrenciesInFeedAndCurrenciesNotInFeed_Then_ReturnsRatesForThoseCurrenciesInFeed()
    {
        var sut = new ExchangeRateProvider(_fixture.MockHttpClient, _fixture.MockSettings);

        // Ask for the rates that are in the test data where the currency code starts with B.
        var currencies = _fixture.CurrenciesInTestData.Where(c => c.Code.StartsWith("B")).ToList();

        var expectedCount = currencies.Count;
        
        currencies.AddRange( new []{new Currency("XYZ"), new Currency("ABC")});
        
        var rates = await sut.GetExchangeRates(currencies, _fixture.MockDate);

        rates.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async Task GivenEmptyResponseFromApi_Then_ReturnNoRates()
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp.When(_fixture.BaseUrl).Respond("application/json", string.Empty);
        
        var sut = new ExchangeRateProvider(mockHttp.ToHttpClient(), _fixture.MockSettings);

        var response = await sut.GetExchangeRates(_fixture.CurrenciesInTestData, _fixture.MockDate);

        response.Should().BeEmpty();
    }
    
    // This test is simply uses a few examples of non-success status codes to demonstrate the behaviour of the method.
    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task GivenNonSuccessResponseFromApi_Then_ReturnNoRates(HttpStatusCode responseCode)
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp.When(_fixture.BaseUrl).Respond(responseCode);
        
        var sut = new ExchangeRateProvider(mockHttp.ToHttpClient(), _fixture.MockSettings);

        var response = await sut.GetExchangeRates(_fixture.CurrenciesInTestData, _fixture.MockDate);

        response.Should().BeEmpty();
    }
}