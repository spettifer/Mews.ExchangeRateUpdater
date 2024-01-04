using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeRateUpdater.Configuration;
using ExchangeRateUpdater.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ExchangeRateUpdater
{
    public class ExchangeRateProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;

        public ExchangeRateProvider(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
        }
        
        /// <summary>
        /// Should return exchange rates among the specified currencies that are defined by the source. But only those defined
        /// by the source, do not return calculated exchange rates. E.g. if the source contains "CZK/USD" but not "USD/CZK",
        /// do not return exchange rate "USD/CZK" with value calculated as 1 / "CZK/USD". If the source does not provide
        /// some of the currencies, ignore them.
        /// </summary>
        public async Task<IEnumerable<ExchangeRate>> GetExchangeRates(IEnumerable<Currency> currencies)
        {
            // https://api.cnb.cz/cnbapi/swagger-ui.html#/
            // https://api.cnb.cz/cnbapi/exrates/daily?date=2019-05-17&lang=EN
            
            // Date could be an argument for this method, but for the purposes of this exercise, I'm going to simply select yesterday's date for now.
            
            var url = new Uri(new Uri(_apiSettings.ExchangeRatesBaseUrl), $"{_apiSettings.DailyRatesEndpoint}?date={DateTime.UtcNow.AddDays(-1):yyyy-M-d}&lang={_apiSettings.LanguageCode}");

            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                var bankRates = JsonConvert.DeserializeObject<DailyRatesResponse>(content);

                return bankRates.Rates.Where(r => currencies.Any(c => c.Code == r.CurrencyCode)).Select(r => r.ToExchangeRate());
            }
            
            return Enumerable.Empty<ExchangeRate>();
        }
    }
}
