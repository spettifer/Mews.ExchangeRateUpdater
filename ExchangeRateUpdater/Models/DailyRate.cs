namespace ExchangeRateUpdater.Models;

public class DailyRate
{
    public int Amount { get; set; }
    public string Country { get; set; }
    public string Currency { get; set; }
    public string CurrencyCode { get; set; }
    public int Order { get; set; }
    public decimal Rate { get; set; }
    public string ValidFor { get; set; }

    public ExchangeRate ToExchangeRate()
    {
        return new ExchangeRate(new Currency("CZK"), new Currency(CurrencyCode), Rate);
    }
}