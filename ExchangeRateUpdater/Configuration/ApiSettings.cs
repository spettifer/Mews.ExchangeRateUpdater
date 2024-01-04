namespace ExchangeRateUpdater.Configuration;

public class ApiSettings
{
    public string LanguageCode { get; set; }
    public string ExchangeRatesBaseUrl { get; set; }
    public string DailyRatesEndpoint { get; set; }
}