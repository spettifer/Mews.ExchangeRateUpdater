using ExchangeRateUpdater.Configuration;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;

namespace ExchangeRateUpdater.Tests.Fixtures;

public class ExchangeRateData : IDisposable
{
    // Include wildcard so that we can match the URL with the query string.

    private const string MockBaseUrl = "http://mock.url/*";
    private readonly string _apiResponse = File.ReadAllText("Fixtures/DailyRateResponse.json");
    
    private readonly MockHttpMessageHandler _mockHttp = new();
    
    public DateTime MockDate { get; } = DateTime.UtcNow.AddDays(-1);

    public ExchangeRateData()
    {
        _mockHttp.When(MockBaseUrl).Respond("application/json", _apiResponse);
    }
    
    public IEnumerable<Currency> CurrenciesInTestData { get; } = new List<Currency>()
    {
        new ("AUD"),
        new ("BRL"),
        new ("BGN"),
        new ("CAD")
    };
    
    public IEnumerable<Currency> CurrenciesNotInTestData { get; } = new List<Currency>()
    {
        new ("GBP"),
        new ("USD")
    };

    public HttpClient MockHttpClient => _mockHttp.ToHttpClient();

    public IOptions<ApiSettings> MockSettings
    {
        get
        {
            var settings = Options.Create(
                new ApiSettings()
                {
                    LanguageCode = "EN",
                    ExchangeRatesBaseUrl = MockBaseUrl,
                    DailyRatesEndpoint = "daily"
                });
            return settings;
        }
    }

    public string BaseUrl => MockBaseUrl;

    public void Dispose()
    {
    }
}