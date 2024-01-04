namespace ExchangeRateUpdater.Tests;

public class ExchangeRateData : IDisposable
{
    public string ApiResponse { get; } = File.ReadAllText("DailyRateResponse.json");

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

    public void Dispose()
    {
    }
}